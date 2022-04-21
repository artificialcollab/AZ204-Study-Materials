# provision a Key Vault resource
New-AzKeyVault -VaultName 'kv-az204-demo02' -ResourceGroupName 'rg-ps-az202' -Location 'Canada Central' 


#enable soft-delete on an existing Vault
($resource = Get-AzResource -ResourceId (Get-AzKeyVault -VaultName 'kv-az204-demo02').ResourceId).Properties | Add-Member -MemberType "NoteProperty" -Name "enableSoftDelete" -Value "true"
Set-AzResource -resourceid $resource.ResourceId -Properties $resource.Properties

#enable purge protection on an existing Vault (once enabled, it can not be disabled)
($resource = Get-AzResource -ResourceId (Get-AzKeyVault -VaultName 'kv-az204-demo02').ResourceId).Properties | Add-Member -MemberType "NoteProperty" -Name "enablePurgeProtection" -Value "true"
Set-AzResource -resourceid $resource.ResourceId -Properties $resource.Properties