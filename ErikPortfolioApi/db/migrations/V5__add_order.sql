﻿ALTER TABLE folder ADD COLUMN folder_order INTEGER NOT NULL DEFAULT 0;
ALTER TABLE photo ADD COLUMN photo_order INTEGER NOT NULL DEFAULT 0;