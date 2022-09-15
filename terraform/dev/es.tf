resource "aws_elasticsearch_domain" "workbc-jb-cluster" {
	domain_name	= "workbc-jb-cluster"
	elasticsearch_version = "OpenSearch_1.3"
	
	cluster_config {
		instance_count = 1
		instance_type = "t3.small.elasticsearch"
	}
	
	node_to_node_encryption {
		enabled = true
	}
	
	encrypt_at_rest {
		enabled = true
	}
	
	domain_endpoint_options {
		enforce_https = true
		tls_security_policy = "Policy-Min-TLS-1-2-2019-07"
	}
	
	ebs_options {
		ebs_enabled = true
		volume_size = 10
		volume_type = "gp3"
		throughput = 125
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

