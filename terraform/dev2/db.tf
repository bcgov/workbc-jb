# RDS

data "aws_db_subnet_group" "data_subnet" {
  name                   = "data-subnet"
}

/*
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
			value = data.aws_iam_role.mssql_native_backup_restore_role.arn
		}
	}
}
*/

# SQL Server
#TODO after db creation
data "aws_db_instance" "mssql" {
  db_instance_identifier = "ceu-mssql"
  /*identifier              = "ceu-mssql"
  allocated_storage       = 300
  max_allocated_storage   = 600
  engine                  = "sqlserver-web"
  engine_version	  = "15.00"
  instance_class          = "db.t3.small"
  username                = local.db_creds.username
  password                = local.db_creds.password
  skip_final_snapshot     = true
  backup_retention_period = 5
  db_subnet_group_name    = data.aws_db_subnet_group.data_subnet.name
#TODO after db creation
  kms_key_id              = data.aws_kms_key.workbc-jb-kms-key.arn
  storage_encrypted       = true
  vpc_security_group_ids  = [data.aws_security_group.data.id, aws_security_group.mssql_security_group.id]
  option_group_name	  = aws_db_option_group.mssql-og.name
  timezone		  = "Pacific Standard Time"
  apply_immediately	  = true
  */
}

# create this manually
#TODO after db creation
data "aws_secretsmanager_secret_version" "creds" {
  secret_id = "workbc-jb-db-creds"
}

locals {
  db_creds = jsondecode(
    data.aws_secretsmanager_secret_version.creds.secret_string
  )
}
