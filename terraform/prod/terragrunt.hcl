terraform {
  source = "./"
}

include {
  path = find_in_parent_folders()
}

locals {
  project = get_env("LICENSE_PLATE")
}

generate "prod_tfvars" {
  path              = "prod.auto.tfvars"
  if_exists         = "overwrite"
  disable_signature = true
  contents          = <<-EOF
    alb_name = "default"
    cloudfront = true
    cloudfront_origin_domain = "workbc-jb.${local.project}-prod.nimbus.cloud.gov.bc.ca"
	cloudfront_origin_domain2 = "workbc-jb-adm.${local.project}-prod.nimbus.cloud.gov.bc.ca"
    service_names = ["workbc-jb"]
	service_names2 = ["workbc-jb-adm"]
  EOF
}
