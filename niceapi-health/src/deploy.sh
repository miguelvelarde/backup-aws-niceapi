#!/bin/bash

echo "Deploying with hardcoded parameters..."

dotnet lambda deploy-serverless \
  --stack-name niceapi-health \
  --region us-east-1 \
  --s3-bucket miguelvelarde-niceapp-deploy \
  --s3-prefix niceapp \
  --template serverless.template \
  --template-parameters "DbConnectionString=\"server=niceapp-db-cluster.cluster-ro-cgb6aikmykd8.us-east-1.rds.amazonaws.com;port=3306;user=admin;password=M3ch3!404;database=nice-db;\""
