
locals {
  common_tags        = var.common_tags
  create_ecs_service = var.app_version == "" ? 0 : 1
  df_conn			       = "Server=jb-babeldb-final.cluster-cchtvqxqwetj.ca-central-1.rds.amazonaws.com;Database=WorkBC_JobBoard;uid=${local.db_creds.username};pwd=${local.db_creds.babelpassword}"
  ent_conn           = "Server=${data.aws_db_instance.mssql.address};Database=WorkBC_Enterprise_NOC;uid=${local.db_creds.username};pwd=${local.db_creds.password}"
  es_conn            = "https://vpc-workbc-jb-cluster-5uxgg4cjbob5isirbe2ykrvwr4.ca-central-1.es.amazonaws.com" 
}
