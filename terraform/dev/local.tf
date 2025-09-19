
locals {
  common_tags        = var.common_tags
  create_ecs_service = var.app_version == "" ? 0 : 1
  df_conn			       = "Server=${aws_rds_cluster.postgres_jbnewfinal.endpoint};Database=jobboard;username=${local.db_creds.username};password=${local.db_creds.newjbpost}"
 # ent_conn           = "Server=${aws_db_instance.mssql.address};Database=WorkBC_Enterprise;uid=${local.db_creds.username};pwd=${local.db_creds.password}"
  es_conn            = "https://${aws_elasticsearch_domain.workbc-jb-cluster.endpoint}"

}
