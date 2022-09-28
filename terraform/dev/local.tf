
locals {
  common_tags        = var.common_tags
  create_ecs_service = var.app_image == "" ? 0 : 1
  df_conn			 = "Server=${aws_db_instance.mssql.address};Database=WorkBC_JobBoard;uid=${local.db_creds.username};pwd=${local.db_creds.password}"
  ent_conn           = "Server=${aws_db_instance.mssql.address};Database=WorkBC_Enterprise;uid=${local.db_creds.username};pwd=${local.db_creds.password}"
}
