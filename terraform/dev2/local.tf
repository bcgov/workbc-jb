
locals {
  common_tags        = var.common_tags
  create_ecs_service = var.app_version == "" ? 0 : 1
  df_conn			       = "Server=${data.aws_db_instance.mssql.address};Database=WorkBC_JobBoard;uid=${local.db_creds.username};pwd=${local.db_creds.password}"
  ent_conn           = "Server=${data.aws_db_instance.mssql.address};Database=WorkBC_Enterprise_NOC;uid=${local.db_creds.username};pwd=${local.db_creds.password}"
  es_conn            = "https://${aws_elasticsearch_domain.workbc-jb2-cluster.endpoint}" 
}
