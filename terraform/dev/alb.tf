# alb.tf

# Use the default ALB that is pre-provisioned as part of the account creation
# This ALB has all traffic on *.LICENSE-PLATE-ENV.nimbus.cloud.gob.bc.ca routed to it
data "aws_alb" "main" {
  name = var.alb_name
}

# Redirect all traffic from the ALB to the target group
data "aws_alb_listener" "front_end" {
  load_balancer_arn = data.aws_alb.main.id
  port              = 443
}

resource "aws_alb_target_group" "app-admin" {
  name                 = "workbc-jb-admin-target-group-${substr(uuid(), 0, 3)}"
  port                 = 8080
  protocol             = "HTTP"
  vpc_id               = module.network.aws_vpc.id
  target_type          = "ip"
  deregistration_delay = 30

  health_check {
    healthy_threshold   = "5"
    interval            = "30"
    protocol            = "HTTPS"
    matcher             = "200"
    timeout             = "5"
    path                = var.health_check_path
    unhealthy_threshold = "2"
    port                = "8080"
  }
    
  lifecycle {
    create_before_destroy = true
    ignore_changes        = [name]
  }

  tags = var.common_tags
}

resource "aws_lb_listener_rule" "host_based_weighted_routing" {
  listener_arn = data.aws_alb_listener.front_end.arn

  action {
    type             = "forward"
    target_group_arn = aws_alb_target_group.app-admin.arn
  }

  condition {
    host_header {
      values = [for sn in var.service_names : "${sn}.*"]
    }
  }
}
