# security.tf

data "aws_vpc" "main" {
	id = module.network.aws_vpc.id
}
	
# ALB Security Group: Edit to restrict access to the application
data "aws_security_group" "web" {
  name = "Web_sg"
}

data "aws_security_group" "app" {
  name = "App_sg"
}

data "aws_security_group" "data" {
  name = "Data_sg"
}

# Traffic to the ECS cluster should only come from the ALB
data "aws_security_group" "ecs_tasks" {
  name        = "workbc-cc-ecs-tasks-security-group"
}


data "aws_security_group" "efs_security_group" {
  name        = "workbc-cc-efs-security-group"
}

resource "aws_security_group" "es_security_group" {
	name	=	"es_sg"
	vpc_id	=	data.aws_vpc.main.id
	
	ingress {
		from_port	=	443
		to_port		=	443
		protocol	=	"tcp"
		cidr_blocks	= [data.aws_vpc.main.cidr_block]
	}
}
	
resource "aws_security_group" "redis_security_group" {
	name	=	"redis_sg"
	vpc_id	=	data.aws_vpc.main.id
	
	ingress {
		from_port	=	6379
		to_port		=	6379
		protocol	=	"tcp"
		cidr_blocks	= [data.aws_vpc.main.cidr_block]
	}
}

resource "aws_security_group" "mssql_security_group" {
	name	=	"mssql_sg"
	vpc_id	=	data.aws_vpc.main.id
	
	ingress {
		from_port	=	1433
		to_port		=	1433
		protocol	=	"tcp"
		cidr_blocks	= [data.aws_vpc.main.cidr_block]
	}
}
