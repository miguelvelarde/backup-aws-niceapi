#!/bin/bash

echo "Deploying with hardcoded parameters..."

# Define the database connection string as a variable
DATABASE_CONNECTION_STRING="server=niceapp-db-cluster.cluster-ro-cgb6aikmykd8.us-east-1.rds.amazonaws.com;port=3306;user=admin;password=M3ch3!404;database=nice-db;"

# Deploy using the variable in template-parameters
dotnet lambda deploy-serverless \
  --stack-name niceapi-auth \
  --region us-east-1 \
  --s3-bucket miguelvelarde-niceapp-deploy \
  --s3-prefix niceapp \
  --template serverless.template \
  --template-parameters "DbConnectionString=\"$DATABASE_CONNECTION_STRING\""

  # Esperar unos segundos para asegurarse de que el despliegue ha finalizado completamente
sleep 10

echo "Obteniendo URL de API Gateway..."
# Obtener los outputs del stack de CloudFormation y filtrar la URL de API Gateway
API_URL=$(aws cloudformation describe-stacks --stack-name niceapi-auth --region us-east-1 --query "Stacks[0].Outputs[?OutputKey=='ApiURL'].OutputValue" --output text)

if [ -n "$API_URL" ]; then
  echo "=================================================="
  echo "✅ Deployment completado exitosamente"
  echo "🌐 URL de API Gateway: $API_URL"
  echo "=================================================="
else
  echo "⚠️ No se pudo obtener la URL de API Gateway"
fi