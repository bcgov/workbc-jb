# ecs.tf

resource "aws_ecs_cluster" "admin" {
  name               = "workbc-jb-admin-cluster"
  capacity_providers = ["FARGATE_SPOT"]

  default_capacity_provider_strategy {
    capacity_provider = "FARGATE_SPOT"
    weight            = 100
  }

  tags = var.common_tags
}

resource "aws_ecs_task_definition" "admin-app" {
  count                    = local.create_ecs_service
  family                   = "workbc-jb-task"
  execution_role_arn       = aws_iam_role.ecs_task_execution_role.arn
  task_role_arn            = aws_iam_role.workbc_jb_container_role.arn
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = var.fargate_cpu
  memory                   = var.fargate_memory
  tags                     = var.common_tags
  /*
  volume {
    name = "contents"
    efs_volume_configuration  {
        file_system_id = aws_efs_file_system.workbc.id
    }
  }
  volume {
    name = "app"
  }*/

  container_definitions = jsonencode([
	{
		essential   = true
		name        = "admin"
		image       = "${var.app_repo}/jb-admin:1.1"
		networkMode = "awsvpc"
		
		logConfiguration = {
			logDriver = "awslogs"
			options = {
				awslogs-create-group  = "true"
				awslogs-group         = "/ecs/${var.app_name}"
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
			{
				name = "ConnectionStrings__EnterpriseConnection",
				value = "${local.ent_conn}"
			},
			{
				name = "ConnectionStrings__ElasticSearchServer",
				value = "${aws_elasticsearch_domain.workbc-jb-cluster.endpoint}"
			},
			{
				name = "ConnectionStrings__Redis",
				value = "${aws_elasticache_replication_group.jb_redis_rg.configuration_endpoint_address}"
			},
			{
				name = "EmailSettings__SmtpServer",
				value = "apps.smtp.gov.bc.ca"
			},
			{
				name = "ASPNETCORE_URLS",
				value = "http://*:8080"
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
/*
		mountPoints = [
			{
				containerPath = "/contents",
				sourceVolume = "contents"
			},
			{
				containerPath = "/app",
				sourceVolume = "app"
			}
		]
		volumesFrom = []
		
		dependsOn = [
			{
				containerName = "init"
				condition = "COMPLETE"
			}
		]*/
	}
  ])
  
  runtime_platform {
    operating_system_family = "LINUX"
    cpu_architecture        = "X86_64"
  }
}

resource "aws_ecs_service" "admin" {
  count                             = local.create_ecs_service
  name                              = "workbc-jb-service"
  cluster                           = aws_ecs_cluster.admin.id
  task_definition                   = aws_ecs_task_definition.admin-app[count.index].arn
  desired_count                     = var.app_count
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
    target_group_arn = aws_alb_target_group.app-admin.id
    container_name   = "admin"
    container_port   = var.app_port
  }

  depends_on = [data.aws_alb_listener.front_end, aws_iam_role_policy_attachment.ecs_task_execution_role]

  tags = var.common_tags
}
