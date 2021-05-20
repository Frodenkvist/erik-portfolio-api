ALTER TABLE photo
DROP CONSTRAINT photo_parent_folder_id_fkey,
ADD CONSTRAINT photo_parent_folder_id_fkey FOREIGN KEY (parent_folder_id) REFERENCES folder(id) ON DELETE CASCADE;
