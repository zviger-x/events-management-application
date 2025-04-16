#!/bin/bash
REPLICA_SET=${MONGO_REPLICA_SET:-rs0}
HOST=${MONGO_HOST:-localhost}
PORT=${MONGO_PORT:-27017}
KEYFILE=${MONGO_KEYFILE:-"defaultkey123"}
ADMIN_USERNAME=${MONGO_INITDB_ROOT_USERNAME:-root}
ADMIN_PASSWORD=${MONGO_INITDB_ROOT_PASSWORD:-rootpassword}
PIDFILE="/tmp/mongo.pid"

# Цвета
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
NC='\033[0m' # Без цвета

echo -e "${BLUE}LOG: Starting MongoDB with replica: $REPLICA_SET. Host: $HOST:$PORT${NC}"

# Подготовка keyfile
echo -e "${GREEN}LOG: Preparing keyfile...${NC}"
echo "$KEYFILE" > /etc/mongo-keyfile
chmod 600 /etc/mongo-keyfile

# Запуск mongod без авторизации
echo -e "${GREEN}LOG: Launching mongod without auth for initial setup...${NC}"
mongod --replSet "$REPLICA_SET" \
  --bind_ip_all \
  --keyFile /etc/mongo-keyfile \
  --logpath /var/log/mongod.log \
  --fork \
  --pidfilepath "$PIDFILE"

# Подождать немного, пока MongoDB полностью запустится
sleep 5

# Проверка инициализации реплика-сета с использованием числового кода ошибки
echo -e "${BLUE}LOG: Checking if replica set is already initialized...${NC}"
REPLICA_STATUS=$(mongosh --host "localhost:$PORT" --quiet --eval "try { rs.status(); print('ok'); } catch(e) { print(e.code); }")

echo -e "${BLUE}LOG: REPLICA_STATUS message: ${REPLICA_STATUS}${NC}"
if [ "$REPLICA_STATUS" == "ok" ] || [ "$REPLICA_STATUS" == "13" ]; then
  echo -e "${BLUE}LOG: Replica set already initialized.${NC}"
else
  # Создаём реплику
  echo -e "${BLUE}LOG: Replica set not initialized (REPLICA_STATUS: '$REPLICA_STATUS'). Proceeding with initialization...${NC}"
  mongosh --host "localhost:$PORT" --eval "
    try {
      rs.initiate({
        _id: '$REPLICA_SET',
        members: [{ _id: 0, host: '$HOST:$PORT' }]
      });
    } catch (e) { print(e); }
  "
  
  # Ждём, пока PRIMARY активен
  echo -e "${GREEN}LOG: Waiting for PRIMARY...${NC}"
  until mongosh --host "localhost:$PORT" --quiet --eval "rs.isMaster().ismaster" | grep true; do
    sleep 1
  done
  
  # Создание администратора
  echo -e "${GREEN}LOG: Creating admin user...${NC}"
  mongosh --host "localhost:$PORT" --eval "
  db.getSiblingDB('admin').createUser({
    user: '$ADMIN_USERNAME',
    pwd: '$ADMIN_PASSWORD',
    roles: [ { role: 'root', db: 'admin' } ]
  });
  "
fi

# Завершаем mongod вручную
echo -e "${GREEN}LOG: Shutting down mongod after setup via PID...${NC}"
if [ -f "$PIDFILE" ]; then
  kill -15 "$(cat $PIDFILE)"
  rm "$PIDFILE"
else
  echo -e "${RED}LOG: PID file not found, skipping shutdown${NC}"
fi

# Подождать завершения
sleep 5

# Перезапуск mongod с авторизацией
echo -e "${BLUE}LOG: Restarting mongod with --auth...${NC}"
exec mongod --replSet "$REPLICA_SET" --bind_ip_all --auth --keyFile /etc/mongo-keyfile
