output "cluster_id" {
  value = aws_eks_cluster.connections.id
}

output "node_group_id" {
  value = aws_eks_node_group.connections.id
}

output "vpc_id" {
  value = aws_vpc.connections_vpc.id
}

output "subnet_ids" {
  value = aws_subnet.connections_subnet[*].id
}
