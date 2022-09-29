data "aws_caller_identity" "current" {}
# ECS task execution role data
data "aws_iam_policy_document" "ecs_task_execution_role" {
  version = "2012-10-17"
  statement {
    sid     = ""
    effect  = "Allow"
    actions = ["sts:AssumeRole"]

    principals {
      type        = "Service"
      identifiers = ["ecs-tasks.amazonaws.com"]
    }
  }
}

# ECS task execution role
resource "aws_iam_role" "ecs_task_execution_role" {
  name               = var.ecs_task_execution_role_name
  assume_role_policy = data.aws_iam_policy_document.ecs_task_execution_role.json

  tags = var.common_tags
}

# ECS task execution role policy attachment
resource "aws_iam_role_policy_attachment" "ecs_task_execution_role" {
  role       = aws_iam_role.ecs_task_execution_role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}

resource "aws_iam_role_policy" "ecs_task_execution_kms" {
  name   = "ecs_task_execution_kms"
  role   = aws_iam_role.ecs_task_execution_role.id
  policy = <<-EOF
  {
    "Version": "2012-10-17",
    "Statement": [
      {
        "Effect": "Allow",
        "Action": [
          "secretsmanager:GetSecretValue",
          "kms:Decrypt"
        ],
        "Resource": [
          "arn:aws:secretsmanager:ca-central-1:873424993519:secret:workbc-jb-db-creds-Ifbm0H",
          "arn:aws:kms:ca-central-1:873424993519:key/5e0a0a1f-e916-4019-a6d6-8f9a8cb1c741"
        ]
      }
    ]
  }
  EOF
}

resource "aws_iam_role_policy" "ecs_task_execution_cwlogs" {
  name = "ecs_task_execution_cwlogs"
  role = aws_iam_role.ecs_task_execution_role.id

  policy = <<-EOF
  {
      "Version": "2012-10-17",
      "Statement": [
          {
              "Effect": "Allow",
              "Action": [
                  "logs:CreateLogGroup"
              ],
              "Resource": [
                  "arn:aws:logs:*:*:*"
              ]
          }
      ]
  }
EOF
}

resource "aws_iam_role" "workbc_jb_container_role" {
  name = "workbc_jb_container_role"

  assume_role_policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": "sts:AssumeRole",
      "Principal": {
        "Service": "ecs-tasks.amazonaws.com"
      },
      "Effect": "Allow",
      "Sid": ""
    }
  ]
}
EOF

  tags = var.common_tags
}

resource "aws_iam_role_policy" "workbc_jb_container_cwlogs" {
  name = "workbc_jb_container_cwlogs"
  role = aws_iam_role.workbc_jb_container_role.id

  policy = <<-EOF
  {
      "Version": "2012-10-17",
      "Statement": [
          {
              "Effect": "Allow",
              "Action": [
                  "logs:CreateLogGroup",
                  "logs:CreateLogStream",
                  "logs:PutLogEvents",
                  "logs:DescribeLogStreams"
              ],
              "Resource": [
                  "arn:aws:logs:*:*:*"
              ]
          }
      ]
  }
  EOF
}

/*
resource "aws_iam_role_policy" "workbc_jb_container_efs" {
  name   = "workbc_jb_container_efs"
  role   = aws_iam_role.workbc_jb_container_role.id
  policy = <<-EOF
  {
      "Version": "2012-10-17",
      "Statement": [
          {
              "Sid": "VisualEditor0",
              "Effect": "Allow",
              "Action": [
                  "elasticfilesystem:ClientMount",
                  "elasticfilesystem:ClientWrite",
                  "elasticfilesystem:ClientRootAccess"
              ],
              "Resource": "*"
          }
      ]
  }
  EOF
}*/

resource "aws_iam_role_policy" "workbc_jb_container_start_task" {
  name = "workbc_jb_container_start_task"
  role = aws_iam_role.workbc_jb_container_role.id

  policy = <<-EOF
  {
      "Version": "2012-10-17",
      "Statement": [
          {
              "Sid": "VisualEditor0",
              "Effect": "Allow",
              "Action": "ecs:StartTask",
              "Resource": "*"
          }
      ]
  }
  EOF  
}

resource "aws_iam_role_policy" "workbc_jb_container_ssm" {
  name = "workbc_jb_container_ssm"
  role = aws_iam_role.workbc_jb_container_role.id

  policy = <<-EOF
  {
      "Version": "2012-10-17",
      "Statement": [
          {
              "Effect": "Allow",
              "Action": [
                  "ssmmessages:CreateControlChannel",
                  "ssmmessages:CreateDataChannel",
                  "ssmmessages:OpenControlChannel",
                  "ssmmessages:OpenDataChannel"
              ],
              "Resource": "*"
          }
      ]
  }
  EOF  
}

/*
resource "aws_iam_role_policy" "workbc_container_ses" {
  name = "workbc_container_ses"
  role = aws_iam_role.workbc_jb_container_role.id

  policy = <<-EOF
  {
      "Version": "2012-10-17",
      "Statement": [
          {
              "Effect": "Allow",
              "Action": [
                  "ses:ListIdentities",
                  "ses:SendEmail"
              ],
              "Resource": [
                  "*"
              ]
          }
      ]
  }
  EOF  
}*/

resource "aws_iam_role" "mssql_native_backup_restore_role" {
	name = "mssqlNativeBackupRestoreRole"
	assume_role_policy = jsonencode({

    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Principal": {
                "Service": "rds.amazonaws.com"
            },
            "Action": "sts:AssumeRole",
            "Condition": {
                "StringEquals": {
					"aws:SourceAccount": "873424993519"
                }
            }
        }
    ]

	})

}

resource "aws_iam_role_policy" "mssql_s3" {
	name = "mssqlS3"
	role = aws_iam_role.mssql_native_backup_restore_role.id
	policy = jsonencode({

    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Action": [
                "s3:ListBucket",
                "s3:GetBucketLocation"
            ],
            "Resource": "arn:aws:s3:::workbc-dev-bucket"
        },
        {
            "Effect": "Allow",
            "Action": [
                "s3:GetObjectAttributes",
                "s3:GetObject",
                "s3:PutObject",
                "s3:ListMultipartUploadParts",
                "s3:AbortMultipartUpload"
            ],
            "Resource": "arn:aws:s3:::workbc-dev-bucket/*"
        }
    ]

	})
}
