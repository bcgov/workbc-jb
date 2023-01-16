# cloudfront.tf

resource "random_integer" "cf_origin_id" {
  min = 1
  max = 100
}

resource "aws_cloudfront_distribution" "workbc-jb" {

  count = var.cloudfront ? 1 : 0

  origin {
    custom_origin_config {
      http_port              = 80
      https_port             = 443
      origin_protocol_policy = "https-only"
      origin_ssl_protocols = [
      "TLSv1.2"]
    }

    domain_name = var.cloudfront_origin_domain
    origin_id   = random_integer.cf_origin_id.result
	
	custom_header {
	  name = "X-Forwarded-Host"
	  value = "api-jobboard.workbc.ca"
	}
	
  }

  enabled         = true
  is_ipv6_enabled = true
  comment         = "JobBoard API"

  default_cache_behavior {
    allowed_methods = [
      "DELETE",
      "GET",
      "HEAD",
      "OPTIONS",
      "PATCH",
      "POST",
    "PUT"]
    cached_methods = ["GET", "HEAD"]

    target_origin_id = random_integer.cf_origin_id.result

    forwarded_values {
      query_string = true
      headers = ["Origin", "Authorization"]

      cookies {
        forward = "all"
      }
    }

    viewer_protocol_policy = "redirect-to-https"
    min_ttl                = 0
    default_ttl            = 86400
    max_ttl                = 31536000
	
    # SimpleCORS
    response_headers_policy_id = "60669652-455b-4ae9-85a4-c4c02393f86c"
  }

  price_class = "PriceClass_100"

  restrictions {
    geo_restriction {
      restriction_type = "none"
    }
  }

  tags = var.common_tags
  
  aliases = ["api-jobboard.workbc.ca"]

  viewer_certificate {
    acm_certificate_arn = "arn:aws:acm:us-east-1:846410483170:certificate/27a09302-3322-4d85-96f3-db4be28a8e24"
    minimum_protocol_version = "TLSv1.2_2021"
    ssl_support_method = "sni-only"
  }
}

output "cloudfront_url" {
  value = "https://${aws_cloudfront_distribution.workbc-jb[0].domain_name}"

}

