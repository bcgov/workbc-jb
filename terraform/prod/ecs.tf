# ecs.tf

resource "aws_ecs_cluster" "jobboard" {
  name               = "workbc-jb-cluster"
  tags = var.common_tags
}
 
resource "aws_ecs_cluster_capacity_providers" "jobboard" {
    cluster_name =  aws_ecs_cluster.jobboard.name
    capacity_providers = ["FARGATE_SPOT"]

    default_capacity_provider_strategy {
      weight            = 100
      capacity_provider = "FARGATE_SPOT"	
  }

  
}

resource "aws_ecs_task_definition" "app" {
  count                    = local.create_ecs_service
  family                   = "workbc-jb-task"
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
		name        = "migration"
		image       = "${var.app_repo}/jb-migration:${var.app_version}"
		networkMode = "awsvpc"
		
		logConfiguration = {
			logDriver = "awslogs"
			options = {
				awslogs-create-group  = "true"
				awslogs-group         = "/ecs/workbc-jobboard-migration"
				awslogs-region        = var.aws_region
				awslogs-stream-prefix = "ecs"
			}
		}		

		environment = [

			{
				name = "ConnectionStrings__MigrationRunnerConnection",
				value = "${local.df_conn}"
			},
			{
				name = "AppSettings__ApplyMigrations",
				value = "true"
			},
			{
				name = "AppSettings__IsProduction",
				value = "true"
			},
			{
				name = "SSOT_URL",
				value = "http://internal-ssot-lb-704773555.ca-central-1.elb.amazonaws.com:3000"
			}

		]

	},
	{
		essential   = true
		name        = "web"
		image       = "${var.app_repo}/jb:${var.app_version}"
		networkMode = "awsvpc"
		
		logConfiguration = {
			logDriver = "awslogs"
			options = {
				awslogs-create-group  = "true"
				awslogs-group         = "/ecs/workbc-jobboard"
				awslogs-region        = var.aws_region
				awslogs-stream-prefix = "ecs"
			}
		}		

		portMappings = [
			{
				hostPort = 8081
				protocol = "tcp"
				containerPort = 8081
			}
		]
		
		environment = [
			{
				name = "ConnectionStrings__DefaultConnection",
				value = "${local.df_conn}"
			},
			#{
				#name = "ConnectionStrings__EnterpriseConnection",
				#value = "${local.ent_conn}"
			#},
			{
				name = "ConnectionStrings__ElasticSearchServer",
				value = "${local.es_conn}"
			},
			{
				name = "ConnectionStrings__Redis",
				value = "${aws_elasticache_replication_group.jb_redis_rg.primary_endpoint_address}"
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
				name = "ASPNETCORE_URLS",
				value = "http://*:8081"
			},
			{
				name = "AppSettings__UseRedisCache",
				value = "true"
			},
			{
				name = "AppSettings__JbAccountUrl",
				value = "https://www.workbc.ca/account"
			},
			{
				name = "AppSettings__JbSearchUrl",
				value = "https://www.workbc.ca/search-and-prepare-job/find-jobs"
			},
			{
				name = "AppSettings__ApplyMigrations",
				value = "false"
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
			},
			{
				name = "AppSettings__GoogleMapsReferrerApi",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:gm_ref::"
			},
			{
				name = "RecaptchaSettings__SiteKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:re_key::"
			},
			{
				name = "RecaptchaSettings__SecretKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:re_secret::"
			},
			{
				name = "TokenManagement__Secret",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:tk_secret::"
			}
		]
		dependsOn = [
			{
				containerName = "migration"
				condition = "COMPLETE"
			}
		]
	},
	{
		essential   = true
		name        = "admin"
		image       = "${var.app_repo}/jb-admin:${var.app_version}"
		networkMode = "awsvpc"
		
		logConfiguration = {
			logDriver = "awslogs"
			options = {
				awslogs-create-group  = "true"
				awslogs-group         = "/ecs/workbc-jobboard-admin"
				awslogs-region        = var.aws_region
				awslogs-stream-prefix = "ecs"
			}
		}		

		portMappings = [
			{
				hostPort = 8080
				protocol = "tcp"
				containerPort = 8080
			}
		]
		
		environment = [
			{
				name = "ConnectionStrings__DefaultConnection",
				value = "${local.df_conn}"
			},
			#{
				#name = "ConnectionStrings__EnterpriseConnection",
				#value = "${local.ent_conn}"
			#},
			{
				name = "ConnectionStrings__ElasticSearchServer",
				value = "${local.es_conn}"
			},
			{
				name = "ConnectionStrings__Redis",
				value = "${aws_elasticache_replication_group.jb_redis_rg.primary_endpoint_address}"
			},
			{
				name = "ASPNETCORE_URLS",
				value = "http://*:8080"
			},
			{
				name = "AppSettings__UseRedisCache",
				value = "true"
			},
			{
				name = "Keycloak__DevModeBypassEnabled",
				value = "false"
			},
			{
				name = "AppSettings__JbAccountUrl",
				value = "https://www.workbc.ca/account"
			},
			{
				name = "AppSettings__JbSearchUrl",
				value = "https://www.workbc.ca/search-and-prepare-job/find-jobs"
			},
			{
				name = "AppSettings__ApplyMigrations",
				value = "false"
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
				name = "Keycloak__Domain",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:kc_domain::"
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
				name = "AzureAdSettings__ClientSecret",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:azure_secret::"
			},
			{
				name = "AzureAdSettings__ClientCertificate",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:azure_certificate::"
			}
		]
		dependsOn = [
			{
				containerName = "migration"
				condition = "COMPLETE"
			}
		]
	},
	{
		essential   = false
		name        = "cli"
		image       = "${var.app_repo}/jb-cli:${var.app_version}"
		networkMode = "awsvpc"
		
		logConfiguration = {
			logDriver = "awslogs"
			options = {
				awslogs-create-group  = "true"
				awslogs-group         = "/ecs/workbc-jobboard-cli"
				awslogs-region        = var.aws_region
				awslogs-stream-prefix = "ecs"
			}
		}		

		environment = [
			{
				name = "ConnectionStrings__DefaultConnection",
				value = "${local.df_conn}"
			},
			#{
				#name = "ConnectionStrings__EnterpriseConnection",
				#value = "${local.ent_conn}"
			#},
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
				name = "ASPNETCORE_URLS",
				value = "http://*:8080"
			},
			{
				name = "AppSettings__UseRedisCache",
				value = "true"
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
				name = "WantedSettings__PassKey",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:wanted_pk::"
			},
			{
				name = "AppSettings__GoogleMapsIPApi",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:gm_ip::"
			},
			{
				name = "FederalSettings__AuthCookie",
				valueFrom = "${data.aws_secretsmanager_secret_version.creds.arn}:fed_auth::"
			}
		]
	}
  ])
  
  runtime_platform {
    operating_system_family = "LINUX"
    cpu_architecture        = "X86_64"
  }
}

resource "aws_ecs_service" "jobboard" {
  count                             = local.create_ecs_service
  name                              = "workbc-jb-service"
  cluster                           = aws_ecs_cluster.jobboard.id
  task_definition                   = aws_ecs_task_definition.app[count.index].arn
  desired_count                     = 2
  enable_ecs_managed_tags           = true
  propagate_tags                    = "TASK_DEFINITION"
  health_check_grace_period_seconds = 60
  wait_for_steady_state             = false
  enable_execute_command            = true


  capacity_provider_strategy {
    capacity_provider = "FARGATE_SPOT"
    weight            = 100
  }


  network_configuration {
    security_groups  = [data.aws_security_group.app.id]
    subnets          = module.network.aws_subnet_ids.app.ids
    assign_public_ip = false
  }

  load_balancer {
    target_group_arn = aws_alb_target_group.web.id
    container_name   = "web"
    container_port   = "8081"
  }
  
  load_balancer {
    target_group_arn = aws_alb_target_group.admin.id
    container_name   = "admin"
    container_port   = var.app_port
  }

  depends_on = [data.aws_alb_listener.front_end, aws_iam_role_policy_attachment.ecs_task_execution_role]

  tags = var.common_tags
}
