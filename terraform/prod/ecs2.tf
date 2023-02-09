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
		image       = "${var.app_repo}/jb-importers-wanted:${var.app_version}"
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
				name = "AppSettings__IsProduction",
				value = "true"
			}
		]
		secrets = [
			
			{
				name = "WantedSettings__PassKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:wanted_pk::"
			}
		]
	},
	{
		essential   = false
		name        = "wanted-indexer"
		image       = "${var.app_repo}/jb-indexers-wanted:${var.app_version}"
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
				name = "AppSettings__IsProduction",
				value = "true"
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
		image       = "${var.app_repo}/jb-importers-federal:${var.app_version}"
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
				name = "AppSettings__IsProduction",
				value = "true"
			}
		]
		secrets = [
			
			{
				name = "AppSettings__GoogleMapsIPApi",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:gm_ip::"
			},
			
			{
				name = "FederalSettings__AuthCookie",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:fed_auth::"
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
		image       = "${var.app_repo}/jb-indexers-federal:${var.app_version}"
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
				name = "AppSettings__IsProduction",
				value = "true"
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
				name = "AppSettings__GoogleMapsIPApi",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:gm_ip::"
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
	# send notification every 6 hours at 7AM, 1PM, 7PM, 1AM PST
	schedule_expression = "cron(0 2,8,14,20 * * ? *)"
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
		image       = "${var.app_repo}/jb-notifications:${var.app_version}"
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
				name = "EmailSettings__UseSes",
				value = "true"
			},
			{
				name = "EmailSettings__FromEmail",
				value = "noreply@workbc.ca"
			},
			{
				name = "AppSettings__JbSearchUrl",
				value = "https://www.workbc.ca/Jobs-Careers/Find-Jobs/Jobs.aspx"
			},
			{
				name = "AppSettings__SendEmailTestingTo",
				value = ""
			},
			{
				name = "AppSettings__IsProduction",
				value = "true"
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
	# send notification every day at 6 AM PST
	schedule_expression = "cron(0 13 * * ? *)"
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
