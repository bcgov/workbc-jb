# Kubernetes Pod Failures — Debug Report

**Date:** 2026-04-29
**Namespace:** `app`
**Cluster:** EKS (ca-central-1)
**Failing pods:** 1 deployment pod + 4 importer-indexer cron pods + 1 notifications cron pod

---

## TL;DR

Six pods across three workloads (`Deployment/jb`, `CronJob/jb-importer-indexer`, `CronJob/jb-notifications`) are stuck in `CreateContainerError`. **The application code is fine** — the containers never start. The kubelet → containerd CRI call fails before the container process is even created.

**Root cause:** the Kubernetes `Secret` named `jb-secret` has a key `AzureAdSettings__ClientCertificate` whose value is **raw binary PFX/PKCS#12 bytes** (starts with `30 82 09 df 02 01 03 …` — the ASN.1 DER prefix). The byte `0x82` at offset 1 is not valid UTF-8.

All affected workloads inject `jb-secret` via `envFrom: secretRef`, which makes every key into a process environment variable. Kubelet ships those env vars to containerd over gRPC, and gRPC mandates valid UTF-8 strings. The CreateContainer RPC therefore fails with:

```
Error: grpc: error while marshaling: string field contains invalid UTF-8
```

The container is never created → `kubectl logs` returns `BadRequest: ... is waiting to start: CreateContainerError`.

---

## Cluster snapshot

```
NAME                                 READY   STATUS                      AGE
jb-5659844bcd-2bj85                  4/4     Running                     42h    <- old ReplicaSet, env baked in before secret broke
jb-6d569c86f-qp8qh                   1/4     CreateContainerError        23h    <- new ReplicaSet, broken
jb-importer-indexer-29623080-d7xc5   0/2     Completed                   27h    <- last successful run
jb-importer-indexer-29623440-88g5m   0/2     Init:CreateContainerError   21h    <- breakage starts here
jb-importer-indexer-29623800-vcxmq   0/2     Init:CreateContainerError   15h
jb-importer-indexer-29624160-46dwd   0/2     Init:CreateContainerError   9h
jb-importer-indexer-29624520-mrjhq   0/2     Init:CreateContainerError   3h11m
jb-notifications-29623020-9mrh8      0/1     Completed                   28h    <- last successful run
jb-notifications-29624460-wcqtm      0/1     CreateContainerError        4h11m
```

**Timing tells the story.** The last importer run that worked was 27 h ago; the next run (21 h ago) failed. Whatever change put binary into `jb-secret` happened in that window. The currently-Running `jb` ReplicaSet (`5659844bcd`) survived only because its env vars were captured at process start *before* the secret was edited. **Any restart of that pod will hit the same error.**

---

## `jb-secret` audit

```
Total keys in jb-secret: 11
OK    AppSettings__GoogleMapsIPApi               (39B)
OK    AppSettings__GoogleMapsReferrerApi         (39B)
BAD   AzureAdSettings__ClientCertificate         (2531B)   <-- offending key
OK    FederalSettings__AuthCookie                (46B)
OK    IndexSettings__ElasticPassword             (11B)
OK    Keycloak__ClientId                         (22B)
OK    Keycloak__ClientSecret                     (32B)
OK    RecaptchaSettings__SecretKey               (40B)
OK    RecaptchaSettings__SiteKey                 (40B)
OK    TokenManagement__Secret                    (98B)
OK    WantedSettings__PassKey                    (32B)
```

First bytes of the bad value: `30 82 09 df 02 01 03 30 82 09 95` — classic ASN.1 SEQUENCE/PFX prefix.

---

## Workloads that mount `jb-secret` via `envFrom` (will all fail on next start)

```
Deployment/jb           containers: web, admin, cli       (cli2 uses jb-secret2 — survives)
CronJob/jb-importer-indexer  containers: federal-importer, wanted-indexer, federal-indexer
CronJob/jb-notifications     containers: notifications
```

The single healthy container in the broken `jb` pod (`cli2`, status `1/4`) uses `jb-secret2`/`jb-config2` — separate Secret, no binary key, no failure. That confirms the diagnosis.

---

## Per-pod logs

### `jb-6d569c86f-qp8qh` (Deployment/jb, new ReplicaSet)

```
$ kubectl logs -n app jb-6d569c86f-qp8qh -c web
Error from server (BadRequest): container "web" in pod "jb-6d569c86f-qp8qh"
  is waiting to start: CreateContainerError

$ kubectl logs -n app jb-6d569c86f-qp8qh -c admin
Error from server (BadRequest): container "admin" in pod "jb-6d569c86f-qp8qh"
  is waiting to start: CreateContainerError

$ kubectl logs -n app jb-6d569c86f-qp8qh -c cli
Error from server (BadRequest): container "cli" in pod "jb-6d569c86f-qp8qh"
  is waiting to start: CreateContainerError

$ kubectl logs -n app jb-6d569c86f-qp8qh -c cli2
(empty — running, no recent output)
```

Events:

```
Warning  Failed   x6403 over 23h   kubelet   Error: grpc: error while marshaling: string field contains invalid UTF-8
Normal   Pulled   x6403 over 23h   kubelet   Container image "...jb:8966c27..." already present on machine
```

(6403 retries in 23 h — pod is in a tight backoff loop.)

### `jb-importer-indexer-29624520-mrjhq` (CronJob, latest run)

The first init container (`wanted-importer`) uses `jb-secret2` and ran successfully; the second init (`federal-importer`) uses `jb-secret` and is stuck.

```
[14:00:30] INFO  1049 records returned by the API
[14:00:32] INFO  28381 total expired IDs returned by API
[14:00:32] INFO  20143 jobs to expire, 8238 skipped (not in system)
[14:01:37] INFO  21 jobs deactivated in Jobs table
[14:01:40] INFO  26 jobs found to import
[14:01:49] INFO  IMPORTER FINISHED — fetched=1049 inserted=27 updated=487 skipped=535
Error from server (BadRequest): container "federal-importer" in pod
  "jb-importer-indexer-29624520-mrjhq" is waiting to start: CreateContainerError
```

Events:

```
Normal   Pulled   x872 over 3h10m   kubelet   Container image "...jb-importers-federal:8966c27..." already present on machine
Warning  Failed   x881 over 3h10m   kubelet   Error: grpc: error while marshaling: string field contains invalid UTF-8
```

The other three importer-indexer pods (`88g5m`, `vcxmq`, `46dwd`) show the identical event pattern.

### `jb-notifications-29624460-wcqtm`

```
$ kubectl logs -n app jb-notifications-29624460-wcqtm
Error from server (BadRequest): container "notifications" in pod
  "jb-notifications-29624460-wcqtm" is waiting to start: CreateContainerError
```

Events:

```
Warning  Failed   x1152 over 4h12m   kubelet   Error: grpc: error while marshaling: string field contains invalid UTF-8
Normal   Pulled   x1151 over 4h12m   kubelet   Container image "...jb-notifications:8966c27..." already present on machine
```

---

## Does updating `jb-secret` require a new release?

**Short answer: no new container image / no new release is required _if_ you keep the value as a UTF-8 string. Just update the Secret and restart the workloads.**

Longer answer, by scenario:

| Approach | Code change? | New image? | Action needed after Secret update |
| --- | --- | --- | --- |
| **Option 2 — base64-encode cert as a string in the Secret** | Yes, the consuming app must `Convert.FromBase64String` before loading the cert | Yes (new build + ECR push + Helm/kustomize bump) | Roll out the new image, then `kubectl rollout restart deployment/jb` |
| **Option 1 — move cert to its own Secret, mount as a file volume** | Probably yes — config has to point at a file path instead of inline cert | Yes (new build) | Apply manifest changes + rollout restart |
| **Option Z — quickest unblock if the cert isn't actually used by importers/notifications** | No | **No** | Remove (or rename to `*Base64`) the bad key in `jb-secret`, then `kubectl rollout restart deployment/jb` and wait for the next CronJob tick (or trigger one manually) |

### Why Kubernetes restarts are still needed

`envFrom` env vars are captured at **container start**. Updating a Secret does **not** propagate to running containers — they keep their original (possibly stale) env values. So:

- **Deployments (`jb`)** — `kubectl rollout restart deployment/jb -n app` to regenerate pods with the new env. No image rebuild needed.
- **CronJobs (`jb-importer-indexer`, `jb-notifications`)** — the next scheduled job will create fresh pods that read the updated Secret. To verify sooner, manually trigger one with `kubectl create job --from=cronjob/<name> <test-name> -n app`.
- **Mounted secret volumes** (not used here) — kubelet refreshes those files on a ~60s clock without restart, but `envFrom` does not.

### What "Option 2 (base64)" actually entails

Option 2 looks easy because changing the Secret is one line. It is **not** zero-code: somewhere in the .NET startup code, the cert is loaded from `IConfiguration["AzureAdSettings:ClientCertificate"]` as raw bytes. After option 2, that value is now a base64 string and the loader has to do `Convert.FromBase64String(cfg)` first. That code change → new image → new release for whichever services consume it (likely `WorkBC.Web` for Azure AD auth).

If the importers/notifications **don't actually use this cert** (only the web app does), the cleanest unblock right now is "Option Z": remove the offending key from `jb-secret` so kubelet can create the importer/notifications containers again, and figure out the proper home for the cert separately. This needs no new release.

**Recommendation:** confirm whether `AzureAdSettings__ClientCertificate` is consumed by anything other than the `web` container. If not → Option Z first to restore service, then Option 1 (mounted volume) properly later.

---

## Step-by-step debugging procedure for this class of failure

Use this whenever you see `CreateContainerError` / `Init:CreateContainerError` and `kubectl logs` is unhelpful.

### 1. Confirm the container never started

```bash
kubectl get pods -n <ns>
kubectl logs -n <ns> <pod> -c <container>
# "BadRequest: ... is waiting to start: CreateContainerError" means no app code ran
```

If logs return `BadRequest: ... is waiting to start`, this is a kubelet/CRI failure, **not** an app failure. Stop reading application code.

### 2. Read the kubelet event for the real reason

```bash
kubectl describe pod -n <ns> <pod>
# Look at the Events: section at the bottom — find the "Warning Failed" line.
```

Common kubelet messages and what they mean:

| Message | Cause |
| --- | --- |
| `grpc: error while marshaling: string field contains invalid UTF-8` | Non-UTF-8 byte in an env var (almost always from a Secret/ConfigMap referenced via `envFrom` or `env.valueFrom`) |
| `failed to reserve container name` | Stuck containerd state; usually transient or needs node drain |
| `Error: secret "foo" not found` / `configmap "bar" not found` | Referenced object missing in the namespace |
| `couldn't find key X in Secret/ConfigMap` | Key listed in `env.valueFrom.secretKeyRef.key` doesn't exist |
| `OOMKilled` (in Last State) | Memory limit too low, container killed during startup |

### 3. If it's the UTF-8 error, audit every `envFrom` source on the pod

```bash
# Show which Secrets/ConfigMaps the pod injects via envFrom:
kubectl get pod -n <ns> <pod> -o json | \
  jq '.spec.containers[]?, .spec.initContainers[]? | {name, envFrom}'
```

For each Secret found, scan it for non-UTF-8 keys:

```bash
kubectl get secret -n <ns> <secret-name> -o json | python3 -c "
import json, sys, base64
d = json.load(sys.stdin)
for k, v in d.get('data', {}).items():
    raw = base64.b64decode(v)
    try:
        raw.decode('utf-8')
        print(f'OK   {k} (len={len(raw)})')
    except UnicodeDecodeError as e:
        print(f'BAD  {k} (len={len(raw)}, first byte 0x{raw[e.start]:02x} at offset {e.start})')
"
```

For ConfigMaps the audit is in the `data` map directly (keys are already strings, but check for `binaryData` too):

```bash
kubectl get configmap -n <ns> <cm-name> -o json | jq '{data: (.data//{}|keys), binaryData: (.binaryData//{}|keys)}'
```

A `binaryData` key referenced via `envFrom` will produce the same UTF-8 error.

### 4. Identify every workload that uses the broken Secret

Do this **before** you mutate the Secret, so you know what to restart:

```bash
kubectl get deploy,statefulset,daemonset,cronjob -n <ns> -o json | python3 -c "
import json, sys
d = json.load(sys.stdin)
for item in d['items']:
    kind, name = item['kind'], item['metadata']['name']
    if kind == 'CronJob':
        spec = item['spec']['jobTemplate']['spec']['template']['spec']
    else:
        spec = item['spec']['template']['spec']
    refs = []
    for c in spec.get('initContainers', []) + spec.get('containers', []):
        for ef in c.get('envFrom') or []:
            if 'secretRef' in ef:    refs.append(('secret',    c['name'], ef['secretRef']['name']))
            if 'configMapRef' in ef: refs.append(('configmap', c['name'], ef['configMapRef']['name']))
    for kind2, cname, ref in refs:
        if ref == '<TARGET_SECRET>':
            print(f'{kind}/{name} container={cname} via {kind2}={ref}')
"
```

### 5. Fix the Secret

Pick one:

**Quickest unblock — drop the bad key (if app doesn't need it from this Secret):**

```bash
# Re-create from a sanitized copy. NEVER pipe binary through `kubectl edit`.
kubectl get secret -n <ns> jb-secret -o json \
  | jq 'del(.data["AzureAdSettings__ClientCertificate"]) | del(.metadata.resourceVersion,.metadata.uid,.metadata.creationTimestamp,.metadata.managedFields)' \
  | kubectl apply -f -
```

**Base64-encode (option 2) — value stays a string, app must decode:**

```bash
# Compute the base64 string once:
B64=$(base64 -w0 < /path/to/cert.pfx)

# Patch the Secret (the value of a Secret data field is itself base64,
# so we are storing the base64 of the cert *inside* a Secret data field — yes,
# that's a double base64; Kubernetes does the outer one for you):
kubectl patch secret -n <ns> jb-secret --type=json \
  -p="[{\"op\":\"replace\",\"path\":\"/data/AzureAdSettings__ClientCertificate\",\"value\":\"$(printf '%s' "$B64" | base64 -w0)\"}]"
```

Then update the consuming app to call `Convert.FromBase64String(...)` before loading the cert, ship a new image, deploy.

**Cleanest — separate Secret + volume mount (option 1):**

```bash
kubectl create secret generic jb-azuread-cert -n <ns> \
  --from-file=client.pfx=/path/to/cert.pfx
```

Then in the Deployment's container spec:

```yaml
volumes:
  - name: azuread-cert
    secret: { secretName: jb-azuread-cert }
containers:
  - name: web
    volumeMounts:
      - name: azuread-cert
        mountPath: /var/secrets/azuread
        readOnly: true
    env:
      - name: AzureAdSettings__ClientCertificatePath
        value: /var/secrets/azuread/client.pfx
```

App code reads the path env var and `new X509Certificate2(File.ReadAllBytes(path), ...)`.

### 6. Restart the right things

```bash
# Deployments — no image change needed if you only fixed the Secret:
kubectl rollout restart deployment/jb -n <ns>
kubectl rollout status  deployment/jb -n <ns>

# CronJobs — wait for the next tick, OR force a run now:
kubectl create job --from=cronjob/jb-importer-indexer jb-imp-test-$(date +%s) -n <ns>
kubectl create job --from=cronjob/jb-notifications     jb-notif-test-$(date +%s) -n <ns>
```

### 7. Verify

```bash
kubectl get pods -n <ns> | grep -Ev 'Running|Completed'   # should be empty
kubectl describe pod -n <ns> <new-pod> | grep -A2 'Events:'   # no "invalid UTF-8" warnings
```

### 8. Cleanup the old failing pods (optional but advised — they spin retries)

```bash
# Delete the stuck CronJob pods so they stop hammering kubelet:
kubectl delete pod -n <ns> -l app=jb-importer-indexer --field-selector=status.phase!=Succeeded
# Or just delete by name:
kubectl delete pod -n <ns> jb-6d569c86f-qp8qh jb-importer-indexer-29623440-88g5m \
  jb-importer-indexer-29623800-vcxmq jb-importer-indexer-29624160-46dwd \
  jb-importer-indexer-29624520-mrjhq jb-notifications-29624460-wcqtm
```

---

## Why the still-running `jb-5659844bcd-2bj85` is dangerous

It hasn't crashed because its env was captured before the Secret broke. **Do not assume it will recover.** Any of the following will trigger the same `CreateContainerError`:

- node drain, eviction, OOM
- HPA scale-up (new replica spawned)
- node autoscaler recycling the underlying EC2
- any rollout (image update, config change, manual restart)

Plan to fix the Secret **before** any of those happen, or you'll lose the web tier.
