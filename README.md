# ECommerce
Simple UI using .net and Api inbuilt for visual search
# SETUP 
 Configure API
 export ANTHROPIC_API_KEY=sk-ant-...
Note: Set your key via an environment variable (recommended — keeps it out of source control):

To run your code

cd src/ECommerce.Web
dotnet user-secrets init
dotnet user-secrets set "Anthropic:ApiKey" "sk-ant-..."

Restore and build without API
cd ECommerce
dotnet restore
dotnet build

Run without API
dotnet run --project src/ECommerce.Web

Open the URL
`http://localhost:5090`

Run the API separately if you want a headless/JSON interface (creates & seeds `ecommerce.db`):
dotnet run --project src/ECommerce.Api

