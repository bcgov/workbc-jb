# scheduled task
resource "aws_ecs_task_definition" "import-job" {
  family                   = "workbc-jb-importer-task"
  execution_role_arn       = aws_iam_role.ecs_task_execution_role.arn
  task_role_arn            = aws_iam_role.workbc_jb_container_role.arn
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = var.fargate_cpu
  memory                   = var.fargate_memory
  tags                     = var.common_tags

  container_definitions = jsonencode([
	{
		essential   = false
		name        = "wanted-importer"
		image       = "${var.app_repo}/jb-importers-wanted:latest"
		networkMode = "awsvpc"
		
		logConfiguration = {
			logDriver = "awslogs"
			options = {
				awslogs-create-group  = "true"
				awslogs-group         = "/ecs/workbc-jobboard-wanted-importer"
				awslogs-region        = var.aws_region
				awslogs-stream-prefix = "ecs"
			}
		}		

		
		environment = [
			{
				name = "ConnectionStrings__DefaultConnection",
				value = "${local.df_conn}"
			},
			{
				name = "ConnectionStrings__EnterpriseConnection",
				value = "${local.ent_conn}"
			},
			{
				name = "ConnectionStrings__ElasticSearchServer",
				value = "${local.es_conn}"
			},
			{
				name = "ConnectionStrings__Redis",
				value = "${aws_elasticache_replication_group.jb_redis_rg.configuration_endpoint_address}"
			},
			{
				name = "EmailSettings__SmtpServer",
				value = "apps.smtp.gov.bc.ca"
			}
		]
		secrets = [
			{
				name = "IndexSettings__ElasticUser",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:es_username::"
			},
			{
				name = "IndexSettings__ElasticPassword",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:es_password::"
			},
			{
				name = "Keycloak__ClientId",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:kc_id::"
			},
			{
				name = "Keycloak__ClientSecret",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:kc_secret::"
			},
			{
				name = "WantedSettings__PassKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:wanted_pk::"
			},
			{
				name = "AppSettings__GoogleMapsIPApi",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:gm_ip::"
			},
			{
				name = "AppSettings__GoogleMapsReferrerApi",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:gm_ref::"
			},
			{
				name = "FederalSettings__AuthCookie",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:fed_auth::"
			},
			{
				name = "EmailSettings__SendGridKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:send_key::"
			},
			{
				name = "EmailSettings__SendGridFromEmail",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:send_email::"
			},
			{
				name = "RecaptchaSettings__SiteKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:re_key::"
			},
			{
				name = "RecaptchaSettings__SecretKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:re_secret::"
			}
		]
	},
	{
		essential   = false
		name        = "wanted-indexer"
		image       = "${var.app_repo}/jb-indexers-wanted:latest"
		networkMode = "awsvpc"
		
		logConfiguration = {
			logDriver = "awslogs"
			options = {
				awslogs-create-group  = "true"
				awslogs-group         = "/ecs/workbc-jobboard-wanted-indexer"
				awslogs-region        = var.aws_region
				awslogs-stream-prefix = "ecs"
			}
		}		

		
		environment = [
			{
				name = "ConnectionStrings__DefaultConnection",
				value = "${local.df_conn}"
			},
			{
				name = "ConnectionStrings__EnterpriseConnection",
				value = "${local.ent_conn}"
			},
			{
				name = "ConnectionStrings__ElasticSearchServer",
				value = "${local.es_conn}"
			},
			{
				name = "ConnectionStrings__Redis",
				value = "${aws_elasticache_replication_group.jb_redis_rg.configuration_endpoint_address}"
			},
			{
				name = "EmailSettings__SmtpServer",
				value = "apps.smtp.gov.bc.ca"
			}
		]
		secrets = [
			{
				name = "IndexSettings__ElasticUser",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:es_username::"
			},
			{
				name = "IndexSettings__ElasticPassword",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:es_password::"
			},
			{
				name = "Keycloak__ClientId",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:kc_id::"
			},
			{
				name = "Keycloak__ClientSecret",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:kc_secret::"
			},
			{
				name = "WantedSettings__PassKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:wanted_pk::"
			},
			{
				name = "AppSettings__GoogleMapsIPApi",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:gm_ip::"
			},
			{
				name = "AppSettings__GoogleMapsReferrerApi",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:gm_ref::"
			},
			{
				name = "FederalSettings__AuthCookie",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:fed_auth::"
			},
			{
				name = "EmailSettings__SendGridKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:send_key::"
			},
			{
				name = "EmailSettings__SendGridFromEmail",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:send_email::"
			},
			{
				name = "RecaptchaSettings__SiteKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:re_key::"
			},
			{
				name = "RecaptchaSettings__SecretKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:re_secret::"
			}
		]
		dependsOn = [
			{
				containerName = "wanted-importer"
				condition = "COMPLETE"
			}
		]
	},
	{
		essential   = false
		name        = "federal-importer"
		image       = "${var.app_repo}/jb-importers-federal:latest"
		networkMode = "awsvpc"
		
		logConfiguration = {
			logDriver = "awslogs"
			options = {
				awslogs-create-group  = "true"
				awslogs-group         = "/ecs/workbc-jobboard-federal-importer"
				awslogs-region        = var.aws_region
				awslogs-stream-prefix = "ecs"
			}
		}		

		
		environment = [
			{
				name = "ConnectionStrings__DefaultConnection",
				value = "${local.df_conn}"
			},
			{
				name = "ConnectionStrings__EnterpriseConnection",
				value = "${local.ent_conn}"
			},
			{
				name = "ConnectionStrings__ElasticSearchServer",
				value = "${local.es_conn}"
			},
			{
				name = "ConnectionStrings__Redis",
				value = "${aws_elasticache_replication_group.jb_redis_rg.configuration_endpoint_address}"
			},
			{
				name = "EmailSettings__SmtpServer",
				value = "apps.smtp.gov.bc.ca"
			}
		]
		secrets = [
			{
				name = "IndexSettings__ElasticUser",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:es_username::"
			},
			{
				name = "IndexSettings__ElasticPassword",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:es_password::"
			},
			{
				name = "Keycloak__ClientId",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:kc_id::"
			},
			{
				name = "Keycloak__ClientSecret",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:kc_secret::"
			},
			{
				name = "WantedSettings__PassKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:wanted_pk::"
			},
			{
				name = "AppSettings__GoogleMapsIPApi",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:gm_ip::"
			},
			{
				name = "AppSettings__GoogleMapsReferrerApi",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:gm_ref::"
			},
			{
				name = "FederalSettings__AuthCookie",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:fed_auth::"
			},
			{
				name = "EmailSettings__SendGridKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:send_key::"
			},
			{
				name = "EmailSettings__SendGridFromEmail",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:send_email::"
			},
			{
				name = "RecaptchaSettings__SiteKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:re_key::"
			},
			{
				name = "RecaptchaSettings__SecretKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:re_secret::"
			}
		]
		dependsOn = [
			{
				containerName = "wanted-indexer"
				condition = "COMPLETE"
			}
		]
	},
	{
		essential   = true
		name        = "federal-indexer"
		image       = "${var.app_repo}/jb-indexers-federal:latest"
		networkMode = "awsvpc"
		
		logConfiguration = {
			logDriver = "awslogs"
			options = {
				awslogs-create-group  = "true"
				awslogs-group         = "/ecs/workbc-jobboard-federal-indexer"
				awslogs-region        = var.aws_region
				awslogs-stream-prefix = "ecs"
			}
		}		

		
		environment = [
			{
				name = "ConnectionStrings__DefaultConnection",
				value = "${local.df_conn}"
			},
			{
				name = "ConnectionStrings__EnterpriseConnection",
				value = "${local.ent_conn}"
			},
			{
				name = "ConnectionStrings__ElasticSearchServer",
				value = "${local.es_conn}"
			},
			{
				name = "ConnectionStrings__Redis",
				value = "${aws_elasticache_replication_group.jb_redis_rg.configuration_endpoint_address}"
			},
			{
				name = "EmailSettings__SmtpServer",
				value = "apps.smtp.gov.bc.ca"
			}
		]
		secrets = [
			{
				name = "IndexSettings__ElasticUser",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:es_username::"
			},
			{
				name = "IndexSettings__ElasticPassword",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:es_password::"
			},
			{
				name = "Keycloak__ClientId",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:kc_id::"
			},
			{
				name = "Keycloak__ClientSecret",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:kc_secret::"
			},
			{
				name = "WantedSettings__PassKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:wanted_pk::"
			},
			{
				name = "AppSettings__GoogleMapsIPApi",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:gm_ip::"
			},
			{
				name = "AppSettings__GoogleMapsReferrerApi",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:gm_ref::"
			},
			{
				name = "FederalSettings__AuthCookie",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:fed_auth::"
			},
			{
				name = "EmailSettings__SendGridKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:send_key::"
			},
			{
				name = "EmailSettings__SendGridFromEmail",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:send_email::"
			},
			{
				name = "RecaptchaSettings__SiteKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:re_key::"
			},
			{
				name = "RecaptchaSettings__SecretKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:re_secret::"
			}
		]
		dependsOn = [
			{
				containerName = "federal-importer"
				condition = "COMPLETE"
			}
		]
	}
  ])
  
  runtime_platform {
    operating_system_family = "LINUX"
    cpu_architecture        = "X86_64"
  }
}

resource "aws_cloudwatch_event_rule" "cron" {
	name = "importer_schedule"
	schedule_expression = "rate(1 hour)"
}

resource "aws_cloudwatch_event_target" "ecs_scheduled_task" {
  arn      = aws_ecs_cluster.jobboard.arn
  rule     = aws_cloudwatch_event_rule.cron.id
  role_arn = aws_iam_role.workbc_jb_events_role.arn

  ecs_target {
    task_count          = 1
    task_definition_arn = trimsuffix(aws_ecs_task_definition.import-job.arn, ":${aws_ecs_task_definition.import-job.revision}")
    launch_type         = "FARGATE"
    network_configuration {
      assign_public_ip = false
      security_groups  = [data.aws_security_group.app.id]
      subnets          = module.network.aws_subnet_ids.app.ids
    }
  }
}

resource "aws_ecs_task_definition" "notify-job" {
  family                   = "workbc-jb-notifications-task"
  execution_role_arn       = aws_iam_role.ecs_task_execution_role.arn
  task_role_arn            = aws_iam_role.workbc_jb_container_role.arn
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = var.fargate_cpu
  memory                   = var.fargate_memory
  tags                     = var.common_tags

  container_definitions = jsonencode([
	{
		essential   = true
		name        = "notifications"
		image       = "${var.app_repo}/jb-notifications:latest"
		networkMode = "awsvpc"
		
		logConfiguration = {
			logDriver = "awslogs"
			options = {
				awslogs-create-group  = "true"
				awslogs-group         = "/ecs/workbc-jobboard-notifications"
				awslogs-region        = var.aws_region
				awslogs-stream-prefix = "ecs"
			}
		}		

		
		environment = [
			{
				name = "ConnectionStrings__DefaultConnection",
				value = "${local.df_conn}"
			},
			{
				name = "ConnectionStrings__EnterpriseConnection",
				value = "${local.ent_conn}"
			},
			{
				name = "ConnectionStrings__ElasticSearchServer",
				value = "${local.es_conn}"
			},
			{
				name = "ConnectionStrings__Redis",
				value = "${aws_elasticache_replication_group.jb_redis_rg.configuration_endpoint_address}"
			},
			{
				name = "EmailSettings__SmtpServer",
				value = "apps.smtp.gov.bc.ca"
			}
		]
		secrets = [
			{
				name = "IndexSettings__ElasticUser",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:es_username::"
			},
			{
				name = "IndexSettings__ElasticPassword",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:es_password::"
			},
			{
				name = "Keycloak__ClientId",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:kc_id::"
			},
			{
				name = "Keycloak__ClientSecret",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:kc_secret::"
			},
			{
				name = "WantedSettings__PassKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:wanted_pk::"
			},
			{
				name = "AppSettings__GoogleMapsIPApi",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:gm_ip::"
			},
			{
				name = "AppSettings__GoogleMapsReferrerApi",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:gm_ref::"
			},
			{
				name = "FederalSettings__AuthCookie",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:fed_auth::"
			},
			{
				name = "EmailSettings__SendGridKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:send_key::"
			},
			{
				name = "EmailSettings__SendGridFromEmail",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:send_email::"
			},
			{
				name = "RecaptchaSettings__SiteKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:re_key::"
			},
			{
				name = "RecaptchaSettings__SecretKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:re_secret::"
			}
		]
	}	
  ])
  
  runtime_platform {
    operating_system_family = "LINUX"
    cpu_architecture        = "X86_64"
  }
}

resource "aws_cloudwatch_event_rule" "cron2" {
	name = "notifications_schedule"
	schedule_expression = "rate(1 day)"
}

resource "aws_cloudwatch_event_target" "ecs_scheduled_task2" {
  arn      = aws_ecs_cluster.jobboard.arn
  rule     = aws_cloudwatch_event_rule.cron2.id
  role_arn = aws_iam_role.workbc_jb_events_role.arn

  ecs_target {
    task_count          = 1
    task_definition_arn = aws_ecs_task_definition.notify-job.arn
    launch_type         = "FARGATE"
    network_configuration {
      assign_public_ip = false
      security_groups  = [data.aws_security_group.app.id]
      subnets          = module.network.aws_subnet_ids.app.ids
    }
  }
}
