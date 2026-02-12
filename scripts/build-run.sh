#!/bin/sh

#building and running observability infrastructures
docker compose -f ../src/observabiity-infrastructure/compose.yaml up -d --build

#building and running microservices
docker compose -f ../src/services/ClojureHighlighter/compose.yaml up -d --build
docker compose -f ../src/services/FortranHighlighter/compose.yaml up -d --build
docker compose -f ../src/services/CsharpHighlighter/compose.yaml up -d --build

#building and running api gatway
docker compose -f ../src/api-gateway/ApiGateway/compose.yaml up -d --build

#building and running client application
docker compose -f ../src/web/compose.yaml up -d --build


