dbProjectPath=Cmb.Database
contextNamespace=Cmb.Database.DbCoffeeMachineContext
startupProjectPath=Cmb.Api

add-migration:
	dotnet ef migrations add $(name) --project "$(dbProjectPath)" --context "$(contextNamespace)" --startup-project "$(startupProjectPath)"
	
apply-migration:
	dotnet ef database update --project "$(dbProjectPath)" --context "$(contextNamespace)" --startup-project "$(startupProjectPath)"