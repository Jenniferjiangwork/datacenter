
DROP procedure IF EXISTS `sp_portfolio_size_insert`;

DELIMITER $$

CREATE PROCEDURE `sp_portfolio_size_insert`()
BEGIN

	DECLARE last_month varchar(2);
	DECLARE last_year varchar(4);

	SET last_month =(select LPAD(MONTH(curdate() - INTERVAL 1 MONTH),2,'0'));
	SET last_year =(select YEAR(curdate() - INTERVAL 1 MONTH));

	DELETE FROM rpt_portfolio_size WHERE year=last_year and month=last_month;

	INSERT INTO rpt_portfolio_size(status,lender,year,month,number,amount,createddate)
	SELECT
		status,
		lender,
		DATE_FORMAT(settleDate,'%Y') as year,
		DATE_FORMAT(settleDate,'%m') as month,
		COUNT(*) as number,
		SUM(balance) as amount,
		curdate() as createddate 
	FROM ams_loanaccount 
	WHERE status in('Repaying', 'Default', 'Recovery', 'Close', 'Settled', 'Written Off') and YEAR(settleDate) = last_year and MONTH(settleDate) = last_month
	
    GROUP BY status,lender 
	ORDER BY lender;
END$$

DELIMITER ;

