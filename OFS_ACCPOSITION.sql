-- Cria o schema staging caso não exista
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'staging')
BEGIN
    EXEC('CREATE SCHEMA [staging]');
END
GO

-- Criação da tabela staging.movimentos
CREATE TABLE [staging].[movimentos] (
    integration_date    DATETIME2       NOT NULL,  -- data e hora da integração
    branch              VARCHAR(14)     NOT NULL,  -- filial
    account_date        DATE            NOT NULL,  -- data contábil
    cosif_account       VARCHAR(13)     NOT NULL,  -- conta COSIF
    internal_account    VARCHAR(13)     NULL,      -- conta interna
    contract_ref        VARCHAR(20)     NULL,      -- referência de contrato
    signal_br           CHAR(1)         NULL,      -- sinal (ex.: D/C)
    amount              DECIMAL(16,2)   NULL,      -- valor
    description         VARCHAR(100)    NULL,      -- descrição
    local_cost_center   VARCHAR(20)     NULL       -- centro de custo local
);
GO
