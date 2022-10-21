resource "aws_s3_bucket" "workbc_jb_s3" {
  bucket = "workbc-jobboard-bucket"
}

resource "aws_s3_bucket_acl" "workbc_s3_acl" {
  bucket = aws_s3_bucket.workbc_jb_s3.id
  acl    = "private"
}

resource "aws_s3_bucket_policy" "allow_access_from_other_accounts" {
  bucket = aws_s3_bucket.workbc_jb_s3.id
  policy = jsonencode({
    "Statement": [
        {
            "Principal": {
                "AWS": "arn:aws:iam::054099626264:role/mssqlNativeBackupRestoreRole"
            },
			"Action": [
                "s3:ListBucket",
                "s3:GetBucketLocation"
            ],
            "Effect": "Allow",
            "Resource": "arn:aws:s3:::workbc-dev-bucket"
        },
        {
            "Principal": {
                "AWS": "arn:aws:iam::054099626264:role/mssqlNativeBackupRestoreRole"
            },
			"Action": [
                "s3:GetObjectAttributes",
                "s3:GetObject",
                "s3:PutObject",
                "s3:ListMultipartUploadParts",
                "s3:AbortMultipartUpload"
            ],
            "Effect": "Allow",
            "Resource": "arn:aws:s3:::workbc-dev-bucket/*"
        }
    ],
    "Version": "2012-10-17"
  })
}

