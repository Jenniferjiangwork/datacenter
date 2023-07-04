
DROP procedure IF EXISTS `sp_portfolio_quality2_insert`;

DELIMITER $$

CREATE PROCEDURE `sp_portfolio_quality2_insert`()
BEGIN

	DECLARE last_month varchar(2);
	DECLARE last_year varchar(4);

	SET last_month =(select LPAD(MONTH(curdate() - INTERVAL 1 MONTH),2,'0'));
	SET last_year =(select YEAR(curdate() - INTERVAL 1 MONTH));

	DELETE FROM rpt_portfolio_quality2 WHERE year=last_year and month=last_month;

	INSERT INTO rpt_portfolio_quality2(status,lender,year,month,number,createddate)
	Select * from(
		SELECT
			attentionstatus as status,
			lender,
			DATE_FORMAT(settleDate,'%Y') as year,
			DATE_FORMAT(settleDate,'%m') as month,
			COUNT(*) as number,
			curdate() as createddate 
		FROM ams_loanaccount 
		WHERE attentionstatus in('Fraud - Possible','Fraud - Confirmed','Debtor - Aggressive','Debtor - Deceptive','Debtor - Passive','Debtor - Free Money','Dispute - General','Dispute - FOS')
		and YEAR(settleDate) = last_year and MONTH(settleDate) = last_month
		GROUP BY lender,attentionstatus
		
		union all
		
		SELECT
			arrangementstatus as status,
			lender,
			DATE_FORMAT(settleDate,'%Y') as year,
			DATE_FORMAT(settleDate,'%m') as month,
			COUNT(*) as number,
			curdate() as createddate 
		FROM ams_loanaccount 
		WHERE arrangementstatus in('Hardship - Received','Hardship - Approved','Hardship - Breached','Arrangement - Approved','Arrangement - Breached') and
		YEAR(settleDate) = last_year and MONTH(settleDate) = last_month
		GROUP BY lender,arrangementstatus
	) as q1
	ORDER BY year desc,month desc;
END$$

DELIMITER ;

