output "resource-group-id" {
  value = data.azurerm_resource_group.reservations-application-group.id
}

output "sql-server-database-id" {
  value = data.azurerm_mssql_server.restaurant-reservations-database.id
}

output "key-vault_uri" {
  value = data.azurerm_key_vault.reservation-app-keyvault.vault_uri
}

output "reservations-storage_account_tier" {
  value = data.azurerm_storage_account.reservations-application-storage-account.account_tier
}

output "client_certificate" {
  value     = azurerm_kubernetes_cluster.kubernetes-cluster.kube_config[0].client_certificate
  sensitive = true
}

output "kube_config" {
  value = azurerm_kubernetes_cluster.kubernetes-cluster.kube_config_raw

  sensitive = true
}
