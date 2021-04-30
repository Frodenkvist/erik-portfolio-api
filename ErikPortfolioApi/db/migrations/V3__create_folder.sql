DELETE FROM photo;

ALTER TABLE photo RENAME path TO physical_path;

CREATE TABLE folder(
	id SERIAL NOT NULL PRIMARY KEY,
	name TEXT NOT NULL,
	parent_folder_id BIGINT
);

ALTER TABLE photo 
ADD COLUMN parent_folder_id BIGINT REFERENCES folder (id),
ADD COLUMN name TEXT NOT NULL
