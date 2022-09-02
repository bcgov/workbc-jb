# RDS

data "aws_db_subnet_group" "data_subnet" {
  name                   = "data-subnet"
#  subnet_ids             = module.network.aws_subnet_ids.data.ids

#  tags = var.common_tags
}

# SQL Server
resource "aws_db_instance" "mssql" {
  allocated_storage       = 10
  max_allocated_storage   = 100
  engine                  = "sqlserver-web"
  instance_class          = "db.t3.small"
  identifier              = "ceu-mssql"
  username                = local.db_creds.username
  password                = local.db_creds.password
  skip_final_snapshot     = true
  backup_retention_period = 5
  db_subnet_group_name    = data.aws_db_subnet_group.data_subnet.name
  kms_key_id              = data.aws_kms_key.workbc-jb-kms-key.arn
  storage_encrypted       = true
  vpc_security_group_ids  = [data.aws_security_group.data.id]
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
