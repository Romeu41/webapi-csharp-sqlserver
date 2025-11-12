/*
=============================================================
  DESAFIO SQL – GLOBALTEC | Autor: Romeu Manoel
  Objetivo: Unificar as informações de contas a pagar e contas pagas
  Banco: SimplePeopleDb
  Data: 12/11/2025
=============================================================
*/

-- Criação do banco de dados
CREATE DATABASE SimplePeopleDb;
GO
USE SimplePeopleDb;
GO

/*============================================================
  TABELAS
============================================================*/

-- Tabela de Pessoas (usada como fornecedores)
CREATE TABLE Pessoas (
    Codigo INT IDENTITY(1,1) PRIMARY KEY,
    Nome VARCHAR(250) NOT NULL,
    CPF VARCHAR(11) NOT NULL,
    DataDeNascimento DATETIME NOT NULL,
    DataDeCriacao DATETIME NOT NULL,
    UF VARCHAR(2) NULL
);

-- Tabela de Usuários
CREATE TABLE Usuario (
    Codigo INT IDENTITY(1,1) PRIMARY KEY,
    Nome VARCHAR(250) NOT NULL,
    Senha VARCHAR(250) NOT NULL,
    DataCriacao DATETIME NOT NULL
);

-- Tabela de Contas a Pagar
CREATE TABLE ContasAPagar (
    Numero BIGINT IDENTITY(1,1) PRIMARY KEY,
    CodigoFornecedor INT NOT NULL,
    DataVencimento DATETIME2(7) NOT NULL,
    DataProrrogacao DATETIME2(7) NULL,
    Valor DECIMAL(18,6) NOT NULL,
    Acrescimo DECIMAL(18,6) NULL,
    Desconto DECIMAL(18,6) NULL,
    FOREIGN KEY (CodigoFornecedor) REFERENCES Pessoas(Codigo)
);

-- Tabela de Contas Pagas
CREATE TABLE ContasPagas (
    Numero BIGINT IDENTITY(1,1) PRIMARY KEY,
    CodigoFornecedor INT NOT NULL,
    DataVencimento DATETIME2(7) NOT NULL,
    DataPagamento DATETIME2(7) NULL,
    Valor DECIMAL(18,6) NOT NULL,
    Acrescimo DECIMAL(18,6) NULL,
    Desconto DECIMAL(18,6) NULL,
    FOREIGN KEY (CodigoFornecedor) REFERENCES Pessoas(Codigo)
);
GO


/*============================================================
  DADOS DE TESTE
============================================================*/

SET DATEFORMAT DMY; -- garante que o formato dia/mês/ano será aceito

INSERT INTO Pessoas (Nome, CPF, DataDeNascimento, DataDeCriacao, UF) VALUES
('Fornecedor Alpha', '11111111111', '10/05/1980', GETDATE(), 'SP'),
('Fornecedor Beta',  '22222222222', '21/09/1985', GETDATE(), 'RJ'),
('Fornecedor Gama',  '33333333333', '11/02/1990', GETDATE(), 'GO'),
('Fornecedor Ômega', '44444444444', '15/03/1993', GETDATE(), 'MG');

INSERT INTO ContasAPagar (CodigoFornecedor, DataVencimento, DataProrrogacao, Valor, Acrescimo, Desconto) VALUES
(1, '15/11/2025', NULL, 1200.50, 0.00, 0.00),
(2, '22/11/2025', '01/12/2025', 900.00, 0.00, 10.00);

INSERT INTO ContasPagas (CodigoFornecedor, DataVencimento, DataPagamento, Valor, Acrescimo, Desconto) VALUES
(3, '25/10/2025', '24/10/2025', 640.90, 0.00, 0.00),
(4, '01/11/2025', '01/11/2025', 970.00, 0.00, 0.00);
GO


/*============================================================
  CONSULTA FINAL (Desafio 2)
============================================================*/
SELECT 
    cp.Numero AS NumeroProcesso,
    p.Nome AS NomeFornecedor,
    p.UF AS UFFornecedor,
    CONVERT(VARCHAR(10), cp.DataVencimento, 103) AS DataVencimento,
    CONVERT(VARCHAR(10), cp.DataPagamento, 103) AS DataPagamento,
    (cp.Valor + ISNULL(cp.Acrescimo,0) - ISNULL(cp.Desconto,0)) AS ValorLiquido,
    'PAGA' AS Situacao
FROM ContasPagas cp
INNER JOIN Pessoas p ON p.Codigo = cp.CodigoFornecedor

UNION ALL

SELECT 
    ap.Numero AS NumeroProcesso,
    p.Nome AS NomeFornecedor,
    p.UF AS UFFornecedor,
    CONVERT(VARCHAR(10), ap.DataVencimento, 103) AS DataVencimento,
    CONVERT(VARCHAR(10), ap.DataProrrogacao, 103) AS DataPagamento,
    (ap.Valor + ISNULL(ap.Acrescimo,0) - ISNULL(ap.Desconto,0)) AS ValorLiquido,
    'A PAGAR' AS Situacao
FROM ContasAPagar ap
INNER JOIN Pessoas p ON p.Codigo = ap.CodigoFornecedor

ORDER BY 
    CAST(CONVERT(VARCHAR(10), DataVencimento, 103) AS DATETIME);
GO
