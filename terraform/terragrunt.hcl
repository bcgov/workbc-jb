locals {
  tfc_hostname     = "app.terraform.io"
  tfc_organization = "bcgov"
  project          = get_env("LICENSE_PLATE")
  environment      = reverse(split("/", get_terragrunt_dir()))[0]
  app_image        = get_env("app_image", "")
  app_repo         = split("/", get_env("app_image"))[0]
}

generate "remote_state" {
  path      = "backend.tf"
  if_exists = "overwrite"
  contents  = <<EOF
terraform {
  backend "remote" {
    hostname = "${local.tfc_hostname}"
    organization = "${local.tfc_organization}"
    workspaces {
      name = "${local.project}-${local.environment}-jb"
    }
  }
}
EOF
}

generate "tfvars" {
  path              = "terragrunt.auto.tfvars"
  if_exists         = "overwrite"
  disable_signature = true
  contents          = <<-EOF
  app_image = "${local.app_image}"
  app_repo = "${local.app_repo}"
EOF
}

generate "provider" {
  path      = "provider.tf"
  if_exists = "overwrite"
  contents  = <<EOF
  provider "aws" {
    region  = var.aws_region
    assume_role {
      role_arn = "arn:aws:iam::$${var.target_aws_account_id}:role/BCGOV_$${var.target_env}_Automation_Admin_Role"
    }
  }
EOF
}
