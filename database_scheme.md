CREATE TABLE bahagian (
  id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  nama_bahagian VARCHAR(255) NOT NULL
);

CREATE TABLE cawangan (
  id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  nama_cawangan VARCHAR(255) NOT NULL,
  id_bahagian INT NOT NULL,
  FOREIGN KEY (id_bahagian) REFERENCES bahagian(id)
);

CREATE TABLE unit (
  id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  nama_unit VARCHAR(255) NOT NULL,
  id_bahagian INT NOT NULL,
  FOREIGN KEY (id_bahagian) REFERENCES bahagian(id)
);

CREATE TABLE stesen (
  id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  nama_stesen VARCHAR(255) NOT NULL,
);

CREATE TABLE kumpulan (
  id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  nama_kumpulan VARCHAR(255) NOT NULL,
);

CREATE TABLE gred (
  id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  abjad VARCHAR(255) NOT NULL,
  nombor VARCHAR(255) NOT NULL,
  pangkat VARCHAR(255),
  gelaran_pangkat VARCHAR(255),
  jabatan VARCHAR(255) NOT NULL,
  id_kumpulan INT,
  FOREIGN KEY (id_kumpulan) REFERENCES kumpulan(id)
);

CREATE TABLE kategori_kursus (
  id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  nama_kategori VARCHAR(255) NOT NULL,
);

CREATE TABLE kursus (
  id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  tajuk VARCHAR(255) NOT NULL,
  tarikh_mula DATE NOT NULL,
  tarikh_akhir DATE NOT NULL,
  lokasi VARCHAR(255) NOT NULL,
  id_kategori INT,
  FOREIGN KEY (id_kategori) REFERENCES kategori_kursus(id)
);

CREATE TABLE users (
  id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  username VARCHAR(255) NOT NULL,
  password VARCHAR(255) NOT NULL,
  user_type VARCHAR(255) NOT NULL,
  id_bahagian INT,
  id_cawangan INT,
  id_unit INT,
  id_stesen INT,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (id_bahagian) REFERENCES bahagian(id),
  FOREIGN KEY (id_cawangan) REFERENCES cawangan(id),
  FOREIGN KEY (id_unit) REFERENCES unit(id),
  FOREIGN KEY (id_stesen) REFERENCES stesen(id)
);

CREATE TABLE pegawai (
  id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  nama_pegawai VARCHAR(255) NOT NULL,
  no_ic VARCHAR(255) NOT NULL,
  id_gred INT,
  id_bahagian INT,
  id_cawangan INT,
  id_unit INT,
  id_stesen INT,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (id_gred) REFERENCES gred(id),
  FOREIGN KEY (id_bahagian) REFERENCES bahagian(id),
  FOREIGN KEY (id_cawangan) REFERENCES cawangan(id),
  FOREIGN KEY (id_unit) REFERENCES unit(id),
  FOREIGN KEY (id_stesen) REFERENCES stesen(id)
);

CREATE TABLE kursus_pegawai (
  id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  tarikh_mula DATE NOT NULL,
  tarikh_akhir DATE NOT NULL,
  id_pegawai INT NOT NULL,
  id_kursus INT NOT NULL,
  jumlah_hari FLOAT NOT NULL,
  bulan INT NOT NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (id_pegawai) REFERENCES pegawai(id),
  FOREIGN KEY (id_kursus) REFERENCES kursus(id)
);