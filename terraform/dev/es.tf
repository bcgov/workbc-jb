resource "aws_elasticsearch_domain" "workbc-jb-cluster" {
	domain_name	= "workbc-jb-cluster"
	elasticsearch_version = "1.3"
	internal_user_database_enabled = true
	
	cluster_config {
		instance_count = 1
		instance_type = t3.small.search
	}
	
	master_user_options {
		master_user_name = local.db_creds.es_username
		master_user_password = local.db_creds.es_password
	}
	
	tags = {
		Domain = "WorkBCJBCluster"
	}
}