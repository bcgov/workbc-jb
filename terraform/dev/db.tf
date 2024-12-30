# RDS

data "aws_db_subnet_group" "data_subnet" {
  name                   = "data-subnet"
}

# Option Group
resource "aws_db_option_group" "mssql-og" {
	name = "ceu-mssql-og"
	option_group_description = "Option for native backup and restore"
	engine_name = "sqlserver-web"
	major_engine_version = "15.00"
	
	option {
		option_name = "SQLSERVER_BACKUP_RESTORE"
		
		option_settings {
			name = "IAM_ROLE_ARN"
			value = aws_iam_role.mssql_native_backup_restore_role.arn
		}
	}
}

# SQL Server
resource "aws_db_instance" "mssql" {
  allocated_storage       = 300
  max_allocated_storage   = 600
  engine                  = "sqlserver-web"
  engine_version	  = "15.00"
  instance_class          = "db.t3.small"
  identifier              = "ceu-mssql"
  username                = local.db_creds.username
  password                = local.db_creds.password
  skip_final_snapshot     = true
  backup_retention_period = 5
  db_subnet_group_name    = data.aws_db_subnet_group.data_subnet.name
  kms_key_id              = data.aws_kms_key.workbc-jb-kms-key.arn
  storage_encrypted       = true
  vpc_security_group_ids  = [data.aws_security_group.data.id, aws_security_group.mssql_security_group.id]
  option_group_name	  = aws_db_option_group.mssql-og.name
  timezone		  = "Pacific Standard Time"
  apply_immediately	  = true
}

# create this manually
data "aws_secretsmanager_secret_version" "creds" {
  secret_id = "workbc-jb-db-creds"
}

locals {
  db_creds = jsondecode(
    data.aws_secretsmanager_secret_version.creds.secret_string
  )
}

# Postgres DB
resource "aws_rds_cluster" "postgres" {
  cluster_identifier      = "jb-postgres-cluster"
  engine                  = "aurora-postgresql"
  engine_version          = "16.4"
# engine_mode		  = "provisioned"
  master_username         = local.db_creds.username
  master_password         = local.db_creds.password
  backup_retention_period = 5
  preferred_backup_window = "07:00-09:00"
  db_subnet_group_name    = data.aws_db_subnet_group.data_subnet.name
  kms_key_id              = data.aws_kms_key.workbc-jb-kms-key.arn
  storage_encrypted       = true
  vpc_security_group_ids  = [data.aws_security_group.data.id]
  skip_final_snapshot     = true
  final_snapshot_identifier = "jb-finalsnapshot"
  
  serverlessv2_scaling_configuration {
    max_capacity = 2.0
    min_capacity = 1.0
  }

  tags = var.common_tags
}

#Babelfish parameter group
resource "aws_rds_cluster_parameter_group" "babelfish_pg" {
	name = "babelfish-pg-group"
	family = "aurora-postgresql16"
	
	parameter {
	   name = "rds.babelfish_status"
	   value = "on"
	   }
	}
	   

#Postgres Babelfish
resource "aws_rds_cluster" "postgres_babelfish" {
  cluster_identifier		 = "jb-babeldb-final"
  engine 			 = "aurora-postgresql"
  engine_version		 = "16.4"
  master_username		 = local.db_creds.username
  master_password     		 = local.db_creds.babelpassword
  backup_retention_period	 = 5
  preferred_backup_window 	 = "07:00-09:00"
  db_subnet_group_name    	 = data.aws_db_subnet_group.data_subnet.name
  kms_key_id              	 = data.aws_kms_key.workbc-jb-kms-key.arn
  storage_encrypted       	 = true
  vpc_security_group_ids 	 = [data.aws_security_group.data.id]
  cluster_parameter_group_name = aws_rds_cluster_parameter_group.babelfish_pg.name
  final_snapshot_identifier 	 = "jbabel-finalsnapshot"
  
  serverlessv2_scaling_configuration {
    max_capacity = 2.0
    min_capacity = 1.0
  }

  tags = var.common_tags
}

resource "aws_rds_cluster_instance" "postgres_babelfish" {
  count = 2
  cluster_identifier = aws_rds_cluster.postgres_babelfish.id
  instance_class     = "db.serverless"
  engine             = aws_rds_cluster.postgres_babelfish.engine
  engine_version     = aws_rds_cluster.postgres_babelfish.engine_version
}



resource "aws_rds_cluster_instance" "postgres" {
  count = 2
  cluster_identifier = aws_rds_cluster.postgres.id
  instance_class     = "db.serverless"
  engine             = aws_rds_cluster.postgres.engine
  engine_version     = aws_rds_cluster.postgres.engine_version
}
