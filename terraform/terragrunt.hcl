locals {
  tfc_hostname     = "app.terraform.io"
  tfc_organization = "bcgov"
  project          = get_env("LICENSE_PLATE")
  environment      = reverse(split("/", get_terragrunt_dir()))[0]
  app_version      = get_env("app_version", "")
  app_repo         = get_env("app_repo", "")
}

generate "remote_state" {
  path      = "backend.tf"
  if_exists = "overwrite"
  contents  = <<EOF
terraform {
  backend "s3" {
    bucket = "terraform-remote-state-${local.project}-${local.environment}"
    key = "workbc-jb.tfstate"
    region = "ca-central-1"
    dynamodb_table = "terraform-remote-state-lock-${local.project}"
    encrypt = true
  }
}
EOF
}

generate "tfvars" {
  path              = "terragrunt.auto.tfvars"
  if_exists         = "overwrite"
  disable_signature = true
  contents          = <<-EOF
  app_repo = "${local.app_repo}"
  app_version = "${local.app_version}"
EOF
}

generate "provider" {
  path      = "provider.tf"
  if_exists = "overwrite"
  contents  = <<EOF
  provider "aws" {
    region  = var.aws_region
  }
EOF
}
