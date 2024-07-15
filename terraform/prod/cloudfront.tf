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
    acm_certificate_arn = "arn:aws:acm:us-east-1:846410483170:certificate/fa77540a-8d73-4a7a-9204-12cddc694d36"
    minimum_protocol_version = "TLSv1.2_2021"
    ssl_support_method = "sni-only"
  }
    # Associate the CloudFront distribution with the existing WAF web ACL by ARN
    # This regulates users' frequent access to the website  
    web_acl_id = "arn:aws:wafv2:us-east-1:846410483170:global/webacl/workbc-jb-protection/23e13da8-b45b-43ae-83cf-eefa2016593a"
}

output "cloudfront_url" {
  value = "https://${aws_cloudfront_distribution.workbc-jb[0].domain_name}"

}

