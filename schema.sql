-- Hotel API — Database Schema
-- MySQL 8.0+ / 9.x

CREATE DATABASE IF NOT EXISTS projetocasa
  DEFAULT CHARACTER SET utf8mb4
  COLLATE utf8mb4_0900_ai_ci;
USE projetocasa;

-- -----------------------------------------------------------------------------
-- utilizadores
-- -----------------------------------------------------------------------------
CREATE TABLE utilizadores (
  id              INT NOT NULL AUTO_INCREMENT,
  nome            VARCHAR(100) NOT NULL,
  email           VARCHAR(150) NOT NULL,
  password_hash   VARCHAR(255) NOT NULL,
  cargo           ENUM('utilizador','suporte','gerente','admin') NOT NULL DEFAULT 'utilizador',
  bio             TEXT,
  telefone        VARCHAR(30),
  imagem_perfil   VARCHAR(255),
  cidade          VARCHAR(100),
  created_at      TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  UNIQUE KEY uk_email (email)
) ENGINE=InnoDB;

-- -----------------------------------------------------------------------------
-- propriedades
-- -----------------------------------------------------------------------------
CREATE TABLE propriedades (
  id                  INT NOT NULL AUTO_INCREMENT,
  id_utilizador       INT NOT NULL,
  titulo              VARCHAR(200) NOT NULL,
  descricao           TEXT,
  preco               DECIMAL(12,2) NOT NULL,
  tipo_propriedade    ENUM('casa','apartamento','terreno','comercial') NOT NULL,
  status_propriedade  ENUM('disponivel','vendida','alugada','inativa') DEFAULT 'disponivel',
  tipo_negocio        ENUM('venda','aluguel'),
  endereco            VARCHAR(255),
  cidade              VARCHAR(100),
  quartos             INT,
  casa_banho          INT,
  area_m2             DECIMAL(10,2),
  created_at          TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  KEY (id_utilizador),
  CONSTRAINT fk_propriedades_user
    FOREIGN KEY (id_utilizador) REFERENCES utilizadores(id)
) ENGINE=InnoDB;

-- -----------------------------------------------------------------------------
-- imagens_propriedades
-- -----------------------------------------------------------------------------
CREATE TABLE imagens_propriedades (
  id              INT NOT NULL AUTO_INCREMENT,
  id_propriedade  INT NOT NULL,
  image_url       VARCHAR(255) NOT NULL,
  PRIMARY KEY (id),
  KEY (id_propriedade),
  CONSTRAINT fk_imagens_propriedade
    FOREIGN KEY (id_propriedade) REFERENCES propriedades(id)
) ENGINE=InnoDB;

-- -----------------------------------------------------------------------------
-- caracteristicas (lookup)
-- -----------------------------------------------------------------------------
CREATE TABLE caracteristicas (
  id    INT NOT NULL AUTO_INCREMENT,
  nome  VARCHAR(50),
  PRIMARY KEY (id)
) ENGINE=InnoDB;

INSERT INTO caracteristicas (nome) VALUES
  ('Garagem'), ('Varanda'), ('Jardim'), ('Piscina'), ('Mobilado');

-- -----------------------------------------------------------------------------
-- propriedades_caracteristicas (N:N)
-- -----------------------------------------------------------------------------
CREATE TABLE propriedades_caracteristicas (
  id                 INT NOT NULL AUTO_INCREMENT,
  id_propriedade     INT,
  id_caracteristica  INT,
  PRIMARY KEY (id),
  KEY (id_propriedade),
  KEY (id_caracteristica),
  CONSTRAINT fk_pc_propriedade
    FOREIGN KEY (id_propriedade) REFERENCES propriedades(id),
  CONSTRAINT fk_pc_caracteristica
    FOREIGN KEY (id_caracteristica) REFERENCES caracteristicas(id)
) ENGINE=InnoDB;

-- -----------------------------------------------------------------------------
-- conversas
-- -----------------------------------------------------------------------------
CREATE TABLE conversas (
  id              INT NOT NULL AUTO_INCREMENT,
  id_propriedade  INT,
  id_enviante     INT NOT NULL,
  id_receptor     INT NOT NULL,
  created_at      TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  KEY (id_propriedade),
  KEY (id_enviante),
  KEY (id_receptor),
  CONSTRAINT fk_conv_propriedade
    FOREIGN KEY (id_propriedade) REFERENCES propriedades(id),
  CONSTRAINT fk_conv_enviante
    FOREIGN KEY (id_enviante) REFERENCES utilizadores(id),
  CONSTRAINT fk_conv_receptor
    FOREIGN KEY (id_receptor) REFERENCES utilizadores(id)
) ENGINE=InnoDB;

-- -----------------------------------------------------------------------------
-- mensagens
-- -----------------------------------------------------------------------------
CREATE TABLE mensagens (
  id                INT NOT NULL AUTO_INCREMENT,
  id_enviado_por    INT NOT NULL,
  id_recebido_por   INT NOT NULL,
  texto_mensagem    TEXT NOT NULL,
  enviado_em        TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  KEY (id_enviado_por),
  KEY (id_recebido_por),
  CONSTRAINT fk_msg_enviado
    FOREIGN KEY (id_enviado_por) REFERENCES utilizadores(id),
  CONSTRAINT fk_msg_recebido
    FOREIGN KEY (id_recebido_por) REFERENCES utilizadores(id)
) ENGINE=InnoDB;

-- -----------------------------------------------------------------------------
-- favoritos (N:N utilizador <-> propriedade)
-- -----------------------------------------------------------------------------
CREATE TABLE favoritos (
  id_utilizador   INT NOT NULL,
  id_propriedade  INT NOT NULL,
  created_at      TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id_utilizador, id_propriedade),
  KEY (id_propriedade),
  CONSTRAINT fk_fav_user
    FOREIGN KEY (id_utilizador) REFERENCES utilizadores(id),
  CONSTRAINT fk_fav_propriedade
    FOREIGN KEY (id_propriedade) REFERENCES propriedades(id)
) ENGINE=InnoDB;

-- -----------------------------------------------------------------------------
-- denuncias
-- -----------------------------------------------------------------------------
CREATE TABLE denuncias (
  id                        INT NOT NULL AUTO_INCREMENT,
  id_denunciante            INT NOT NULL,
  id_utilizador_denunciado  INT,
  id_propriedade            INT,
  razao                     TEXT NOT NULL,
  created_at                TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  KEY (id_denunciante),
  KEY (id_utilizador_denunciado),
  KEY (id_propriedade),
  CONSTRAINT fk_den_denunciante
    FOREIGN KEY (id_denunciante) REFERENCES utilizadores(id),
  CONSTRAINT fk_den_denunciado
    FOREIGN KEY (id_utilizador_denunciado) REFERENCES utilizadores(id),
  CONSTRAINT fk_den_propriedade
    FOREIGN KEY (id_propriedade) REFERENCES propriedades(id)
) ENGINE=InnoDB;
