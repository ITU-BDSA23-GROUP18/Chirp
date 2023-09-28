#!/usr/bin/env bash
sqlite3 ../data/cheepDatabase.db < data/schema.sql
sqlite3 ../data/cheepDatabase.db < data/dump.sql