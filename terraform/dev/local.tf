
locals {
  common_tags        = var.common_tags
  create_ecs_service = var.app_version == "" ? 0 : 1
  df_conn			       = "Server=jb-babelr1-instance-1.cchtvqxqwetj.ca-central-1.rds.amazonaws.com;Database=WorkBC_JobBoard;uid=${local.db_creds.username};pwd=${local.db_creds.babelpassword}"
  ent_conn           = "Server=${aws_db_instance.mssql.address};Database=WorkBC_Enterprise;uid=${local.db_creds.username};pwd=${local.db_creds.password}"
  es_conn            = "https://${aws_elasticsearch_domain.workbc-jb-cluster.endpoint}" 

}
