-- Dedicated database user for Feudal.Tools (Tradepost, Livemap, statistics) - never give it the root account.
-- Replace FT_USER and CHOOSE_A_LONG_PASSWORD, then run:  mysql -uroot -p < feudaltools_grants.sql
-- Port 3306 must be forwarded to the server PC; restrict it to the Feudal.Tools address if the router allows it.

CREATE USER IF NOT EXISTS 'FT_USER'@'%' IDENTIFIED BY 'CHOOSE_A_LONG_PASSWORD';

-- read the world (players, items, buildings, map) for livemap, statistics and the trade window
GRANT SELECT ON lif_1.* TO 'FT_USER'@'%';

-- write only the mod's own tables (MariaDB has no table wildcards; the tables exist after the first boot with the mods)
GRANT INSERT, UPDATE, DELETE ON lif_1.nyu_tradeposts             TO 'FT_USER'@'%';
GRANT INSERT, UPDATE, DELETE ON lif_1.nyu_tradepost_offers       TO 'FT_USER'@'%';
GRANT INSERT, UPDATE, DELETE ON lif_1.nyu_tradepost_market       TO 'FT_USER'@'%';
GRANT INSERT, UPDATE, DELETE ON lif_1.nyu_tradepost_transfers    TO 'FT_USER'@'%';
GRANT INSERT, UPDATE, DELETE ON lif_1.nyu_tradepost_transactions TO 'FT_USER'@'%';
GRANT INSERT, UPDATE, DELETE ON lif_1.nyu_tradepost_log          TO 'FT_USER'@'%';
GRANT INSERT, UPDATE, DELETE ON lif_1.nyu_ttmod_tokens           TO 'FT_USER'@'%';
GRANT INSERT, UPDATE, DELETE ON lif_1.nyu_rcon_schedule          TO 'FT_USER'@'%';
GRANT INSERT, UPDATE, DELETE ON lif_1.nyu_rcon_eventlog          TO 'FT_USER'@'%';

FLUSH PRIVILEGES;
