terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.0"
    }
  }
}


provider "azurerm" {
  features {}
}

data "azurerm_resource_group" "reservations-application-group" {
  name = "reservations-application-group"
}

data "azurerm_mssql_server" "restaurant-reservations-database" {
  name                = "restaurant-reservations"
  resource_group_name = data.azurerm_resource_group.reservations-application-group.name
}

data "azurerm_key_vault" "reservation-app-keyvault" {
  name                = "reservationappkeyvault"
  resource_group_name = data.azurerm_resource_group.reservations-application-group.name
}

data "azurerm_storage_account" "reservations-application-storage-account" {
  name                = "reservationappstorage"
  resource_group_name = data.azurerm_resource_group.reservations-application-group.name
}

resource "azurerm_kubernetes_cluster" "kubernetes-cluster" {
  name                = "reservations-api-cluster"
  location            = "southcentralus"
  resource_group_name = data.azurerm_resource_group.reservations-application-group.name
  dns_prefix          = "reservations"
  sku_tier            = "Free"

  default_node_pool {
    name       = "default"
    node_count = 1
    vm_size    = "Standard_D2_v2"
  }

  identity {
    type = "SystemAssigned"
  }

  tags = {
    Environment = "Development"
  }
}






