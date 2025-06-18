#!/bin/bash
REPLICA_SET=${MONGO_REPLICA_SET:-rs0}
HOST=${MONGO_HOST:-localhost}
PORT=27017
KEYFILE=${MONGO_KEYFILE:-"defaultkey123"}
ADMIN_USERNAME=${MONGO_INITDB_ROOT_USERNAME:-root}
ADMIN_PASSWORD=${MONGO_INITDB_ROOT_PASSWORD:-rootpassword}

PIDFILE="/tmp/mongo.pid"
NOAUTH_HOST="localhost"

# �����
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
NC='\033[0m' # ��� �����

echo -e "${BLUE}Starting MongoDB with replica: $REPLICA_SET. Host: $HOST:$PORT${NC}"

# ���������� keyfile
echo -e "${GREEN}Preparing keyfile...${NC}"
echo "$KEYFILE" > /etc/mongo-keyfile
chmod 600 /etc/mongo-keyfile

# ������ mongod ��� �����������
echo -e "${GREEN}Launching mongod without auth for initial setup...${NC}"
mongod --replSet "$REPLICA_SET" \
  --bind_ip_all \
  --keyFile /etc/mongo-keyfile \
  --logpath /var/log/mongod.log \
  --fork \
  --pidfilepath "$PIDFILE" \
  --port "$PORT"

# ��������� �������, ���� MongoDB ��������� ����������
sleep 5

# �������� ������������� �������-���� � �������������� ��������� ���� ������
echo -e "${BLUE}Checking if replica set is already initialized...${NC}"
REPLICA_STATUS=$(mongosh --host "$NOAUTH_HOST:$PORT" --quiet --eval "try { rs.status(); print('ok'); } catch(e) { print(e.code); }")

echo -e "${BLUE}REPLICA_STATUS message: ${REPLICA_STATUS}${NC}"
if [ "$REPLICA_STATUS" == "ok" ] || [ "$REPLICA_STATUS" == "13" ]; then
  echo -e "${BLUE}Replica set already initialized.${NC}"
else
  # ������ �������
  echo -e "${BLUE}Replica set not initialized (REPLICA_STATUS: '$REPLICA_STATUS'). Proceeding with initialization...${NC}"
  mongosh --host "$NOAUTH_HOST:$PORT" --quiet --eval "
    try {
      rs.initiate({
        _id: '$REPLICA_SET',
        members: [{ _id: 0, host: '$HOST:$PORT' }]
      });
    } catch (e) {
      print('Error: ' + e.message);
    }
  "
  
  # ���, ���� PRIMARY �������
  echo -e "${GREEN}Waiting for PRIMARY...${NC}"
  until mongosh --host "$NOAUTH_HOST:$PORT" --quiet --eval "rs.isMaster().ismaster" | grep true; do
    sleep 1
  done
  
  # �������� ��������������
  echo -e "${GREEN}Creating admin user...${NC}"
  mongosh --host "$NOAUTH_HOST:$PORT" --quiet --eval "
    try {
      db.getSiblingDB('admin').createUser({
        user: '$ADMIN_USERNAME',
        pwd: '$ADMIN_PASSWORD',
        roles: [{ role: 'root', db: 'admin' }]
      });
      print('created');
    } catch (e) {
      print('Error: ' + e.message);
    }
  "
fi

# ��������� mongod �������
echo -e "${GREEN}Shutting down mongod after setup via PID...${NC}"
if [ -f "$PIDFILE" ]; then
  kill -15 "$(cat $PIDFILE)"
  rm "$PIDFILE"
else
  echo -e "${RED}PID file not found, skipping shutdown${NC}"
fi

# ��������� ����������
sleep 5

# ���������� mongod � ������������
echo -e "${BLUE}Restarting mongod with --auth...${NC}"
exec mongod --replSet "$REPLICA_SET" --bind_ip_all --auth --keyFile /etc/mongo-keyfile --port "$PORT"
