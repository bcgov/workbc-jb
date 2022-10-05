# Azure VM Setup Part 1 -- Elasticsearch

1. Turn off IE Enhanced Security Configuration on all 3 servers.
2. Install Chrome
3. Download Elasticsearch for Windows [https://www.elastic.co/downloads/elasticsearch](https://www.elastic.co/downloads/elasticsearch)
4. Unzip into E:\elasticsearch
5. Open Windows firewall ports

    ```
    netsh advfirewall firewall add rule name=ECS-Kibana-Inbound-9200-9300-5601 dir=in action=allow protocol=TCP localport=9200,9300,5601 
    ```

6. Install Notepad++
7. Edit E:\elasticsearch\config\elasticsearch.yml
    1. See sample files.  elasticsearch.AANG.yml, elasticsearch.APPA.yml, elasticsearch.MOMO.yml
    2. Edit these 5 lines in the files and save as elasticsearch.yml on each server (also remove "#" if the line is commented out)

    ```
    cluster.name: job-board
    network.host: 0.0.0.0    
    node.name: appa
    discovery.seed_hosts: ["appa", "momo", "aang"]
    cluster.initial_master_nodes: ["appa", "momo", "aang"]
    ```
    3. Add these line to the end of the file (the TBTB proxy causes an error with the GEOIP database update, and we don't use GEOIP features anyway)

    ```
    # Don't update GEOIP databases (the TBTB proxy causes an error)
    ingest.geoip.downloader.enabled: false
    ```
    

8. Created a folder E:\elasticsearch\config\analysis
    1. Put the synonym.txt file in the folder (it can be found in the Build\Elastic folder of the JobBoard git repository)
    2. **Note: There is a TFS action that deploys this file.  It may need to be updated for the cluster config.**
    3. **Note: This feature has been disabled for AWS config. It uses WorkBC/ElasticSearch.Indexing/Resources/synonym_predefined.json instead.**
9. Open command prompt as administrator
    1. E:\elasticsearch\bin>elasticsearch-service.bat install
    2. E:\elasticsearch\bin>elasticsearch-service.bat manager
        1. Set Java initial memory pool and Maximum memory pool to 4096 MB _(We have it set to 4096 MB on Katara, which should be considered a baseline minimum for prod)_
        2. Set startup type to automatic
    3. E:\elasticsearch\bin>elasticsearch-service.bat start
10. Make sure you can connect to [http://localhost:9200/](http://localhost:9200/) in Chrome
11. When you go to [http://localhost:9200](http://localhost:9200) on all 3 servers, the cluster_uuid should be the same
    1. If the uuid is different, stop elasticsearch and delete everything from E:\elasticsearch\data\nodes, then restart elasticsearch. 
    2. Use bin\elasticsearch.bat to start elasticsearch from the console if you need more debugging info.


# Azure VM Setup Part 2 -- Redis

These instructions are based on https://medium.com/a-layman/introduction-to-redis-replication-and-redis-sentinels-on-windows-2bfa9e89681d

1. Download Redis 3.2.100 for Windows and install it using the MSI installer [https://github.com/microsoftarchive/redis/releases](https://github.com/microsoftarchive/redis/releases)
    1. In the installer wizard, set the Max Memory limit to 1024 MB.
    2. Set the folder to E:\Redis
    3. Select "Add the Redis installation folder to the PATH environment variable"

    You can use redis-cli to make sure Redis is working
    ```
    C:\Program Files\Redis>redis-cli
    127.0.0.1:6379> SET hkey "Hello World!"
    OK
    127.0.0.1:6379> GET hkey
    "Hello World!"
    127.0.0.1:6379>
    ```

2. These are our 3 servers. We'll call aang (aka prod2) the master.  

    ```
    aang   142.34.243.70 (master)
    appa   142.34.243.68 (slave) 
    momo   142.34.243.69 (slave)
    ```

3. Edit E:\redis.windows-service.conf and set the "bind" line to include both 127.0.0.1 and the server IP 

    aang
    ```
    bind 127.0.0.1 142.34.243.70
    ```

    appa
    ```
    bind 127.0.0.1 142.34.243.68
    ```

    momo
    ```
    bind 127.0.0.1 142.34.243.69
    ```

3. On the 2 slave servers, add this line to redis.windows-service.conf

    ```
    slaveof 142.34.243.70 6379
    ```

4. Add firewall rules

    ```
    netsh advfirewall firewall add rule name=Redis-Inbound-6379-26379 dir=in action=allow protocol=TCP localport=6379,26379
    ```

5. Create a new file called redis.sentinel.conf

    ```
    protected-mode no
    port 26379

    sentinel monitor mymaster 142.34.243.70 6379 2
    sentinel down-after-milliseconds mymaster 60000
    sentinel failover-timeout mymaster 180000
    sentinel parallel-syncs mymaster 1
    ```
    Copy the redis.sentinel.conf to all 3 servers

6. Install the sentinel service

    ```
    redis-server --service-install redis.sentinel.conf --sentinel --service-name "Redis Sentinel"
    ```

7. Restart the Redis Service

8. Start Redis sentinel services on all 3 machines

9. Make sure the sentinel is working 

    ```
    redis-cli -h 127.0.0.1 -p 26379 sentinel slaves mymaster
    ```

10. Add a release environment variable in TFS for the prod servers so the .NET Core Redis client connects to the first available sentinel instead of connecting directly to Redis

    Prod Server 1 (appa): `ConnectionStrings.Redis = appa,aang,momo,serviceName=mymaster`
	
    Prod Server 2 (aang): `ConnectionStrings.Redis = aang,momo,appa,serviceName=mymaster`
	
    Prod Server 3 (momo): `ConnectionStrings.Redis = momo,appa,aang,serviceName=mymaster`