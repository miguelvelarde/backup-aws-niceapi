#!/bin/bash

RED='\033[0;31m'
GREEN='\033[0;32m'
NC='\033[0m' # No Color

STACK_NAME="niceapi-users"
REGION="us-east-1"
BUCKET_NAME="miguelvelarde-niceapp-deploy"
S3_PREFIX="niceapp"
TEMPLATE_FILE="template.yaml"

echo -e "${GREEN}Deleting previous stack: $STACK_NAME...${NC}"
aws cloudformation delete-stack --stack-name $STACK_NAME --region $REGION
if [ $? -ne 0 ]; then
  echo -e "${RED}Error deleting stack $STACK_NAME. Please check the stack name and region.${NC}"
  exit 1
fi
echo -e "${GREEN}Waiting for stack deletion to complete...${NC}"
aws cloudformation wait stack-delete-complete --stack-name $STACK_NAME --region $REGION

echo -e "${GREEN}✅ Stack $STACK_NAME deleted successfully.${NC}"

echo -e "${GREEN}🛠️ Ejecutando sam build...${NC}"
sam build

echo -e "${GREEN}Deploying Users Lambda function with SAM...${NC}"
sam deploy \
  --no-resolve-s3 \
  --stack-name $STACK_NAME \
  --region $REGION \
  --s3-bucket $BUCKET_NAME \
  --s3-prefix $S3_PREFIX \
  --template-file $TEMPLATE_FILE \
  --capabilities CAPABILITY_IAM \
  --no-fail-on-empty-changeset


echo -e "${GREEN}✅ Deployment completado exitosamente${NC}"