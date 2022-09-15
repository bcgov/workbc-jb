resource "aws_elasticsearch_domain" "workbc-jb-cluster" {
	domain_name	= "workbc-jb-cluster"
	elasticsearch_version = "OpenSearch_1.3"
	
	cluster_config {
		instance_count = 1
		instance_type = "t3.small.elasticsearch"
	}
	
	advanced_security_options {
		enabled = true
		internal_user_database_enabled = true
		master_user_options {
			master_user_name = local.db_creds.es_username
			master_user_password = local.db_creds.es_password
		}
	}
	
	tags = {
		Domain = "WorkBCJBCluster"
	}
}
