resource "aws_s3_bucket" "workbc_jb_s3" {
  bucket = "workbc-jobboard-bucket"
}

resource "aws_s3_bucket_acl" "workbc_s3_acl" {
  bucket = aws_s3_bucket.workbc_jb_s3.id
  acl    = "private"
}

resource "aws_s3_bucket_policy" "allow_access_from_other_accounts" {
  bucket = aws_s3_bucket.workbc_jb_s3.id
  policy = data.aws_iam_policy_document.allow_access_from_other_accounts.json
}

data "aws_iam_policy_document" "allow_access_from_other_accounts" {
  statement {
    principals {
      type        = "AWS"
      identifiers = ["arn:aws:iam::054099626264:role/mssqlNativeBackupRestoreRole"]
    }

    actions = [
      "s3:ListBucket",
      "s3:GetBucketLocation",
    ]

    resources = [
      aws_s3_bucket.example.arn,
      "${aws_s3_bucket.workbc_jb_s3.arn}/*",
    ]
  }
	
  statement {
    principals {
      type        = "AWS"
      identifiers = ["arn:aws:iam::054099626264:role/mssqlNativeBackupRestoreRole"]
    }

    actions = [
      "s3:GetObjectAttributes",
      "s3:GetObject",
      "s3:PutObject",
      "s3:ListMultipartUploadParts",
      "s3:AbortMultipartUpload",
    ]

    resources = [
      aws_s3_bucket.example.arn,
      "${aws_s3_bucket.workbc_jb_s3.arn}/*",
    ]
  }
}

