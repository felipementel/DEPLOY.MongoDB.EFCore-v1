  mongo-express-service:
    image: mongo-express:1.0.2-20-alpine3.19
    container_name: mongoexpress-container
    restart: always
    ports:
      - 8077:8081
    depends_on:
      - mongodb-service
    environment:
      ME_CONFIG_BASICAUTH_ENABLED: false
      ME_CONFIG_BASICAUTH: false
      ME_CONFIG_MONGODB_URL: mongodb://${ADMIN_MONGODB_USER}:${ADMIN_MONGODB_PASSWORD}@mongodb-container:27017/?authSource=admin
      ME_CONFIG_MONGODB_ENABLE_ADMIN: "true"
      ME_CONFIG_OPTIONS_EDITORTHEME: "dracula"
      ME_CONFIG_SITE_BASEURL: '/'
      ME_CONFIG_HEALTH_CHECK_PATH: '/status'
    deploy:
      resources:
        limits:
          cpus: '0.50'
          memory: 256M
        reservations:
          cpus: '0.25'
          memory: 128M
    networks:
      - canal-deploy-network
