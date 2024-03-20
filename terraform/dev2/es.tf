data "aws_region" "current" {}

data "aws_iam_roles" "es-role" {
  name_regex = "AWSServiceRoleForAmazonElasticsearchService"
}

locals {
  es_role_exists = can(data.aws_iam_roles.es-role.arns)
}

resource "aws_iam_service_linked_role" "es-noc" {
	count = local.es_role_exists ? 0 : 1
	aws_service_name = "es.amazonaws.com"
}


resource "aws_elasticsearch_domain" "workbc-jb2-cluster" {
	domain_name	= "workbc-jb2-cluster"
	elasticsearch_version = "OpenSearch_1.3"
	
	cluster_config {
		instance_count = 1
		instance_type = "t3.small.elasticsearch"
		zone_awareness_enabled = false
	}
	
	vpc_options {
		subnet_ids = [
			sort(module.network.aws_subnet_ids.app.ids)[0]
		]

		security_group_ids = [data.aws_security_group.es_security_group.id]
	}
	
	advanced_options = {
		"rest.action.multi.allow_explicit_index" = "true"
	}
	
	access_policies = <<EOF
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Action": "es_noc:*",
            "Principal": "*",
            "Effect": "Allow",
            "Resource": "arn:aws:es:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:domain/workbc-jb2-cluster/*"
        }
    ]
}
EOF
	
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
		Domain = "WorkBCJBNOCCluster"
	}
	
	depends_on = [aws_iam_service_linked_role.es-noc]
}

