
DROP procedure IF EXISTS `sp_portfolio_quality1_loantype_insert`;

DELIMITER $$

CREATE PROCEDURE `sp_portfolio_quality1_loantype_insert`()
BEGIN

	DECLARE last_month varchar(2);
	DECLARE last_year varchar(4);
	
	CREATE TEMPORARY TABLE IF NOT EXISTS tempLoanType
	(
		lender varchar(45),
		numbr int
	);

	SET last_month =(select LPAD(MONTH(curdate() - INTERVAL 1 MONTH),2,'0'));
	SET last_year =(select YEAR(curdate() - INTERVAL 1 MONTH));

	DELETE FROM rpt_portfolio_quality1_loantype WHERE year=last_year and month=last_month;

	INSERT INTO tempLoanType
	(SELECT lender,count(*) FROM ams_loanaccount where loantype in('P And I','FLEX','IO') and
		YEAR(settleDate) = last_year and MONTH(settleDate) = last_month group by lender);

	BEGIN
		DECLARE tempnumb int;
		DECLARE templender varchar(45);
		DECLARE done int DEFAULT FALSE;
		DECLARE cur_loantype CURSOR for (select * from tempLoanType);
		
		DECLARE CONTINUE HANDLER FOR NOT FOUND SET done=TRUE;
		OPEN cur_loantype;
		
		READ_LOOP: LOOP
			FETCH cur_loantype INTO templender,tempnumb;
			IF done THEN
				LEAVE READ_LOOP;
			END IF;
		
			INSERT INTO rpt_portfolio_quality1_loantype(loantype,lender,year,month,number,percent,createddate)
			select * from (
				select 
					loantype,
					lender,
					DATE_FORMAT(settleDate,'%Y') as year,
					DATE_FORMAT(settleDate,'%m') as month,
					COUNT(*) as number,
					(case when loantype ='P And I' then (count(*)/tempnumb * 100) 
						 when loantype ='FLEX' then (count(*)/tempnumb * 100) 
						 when loantype ='IO' then  (count(*)/tempnumb * 100) end) as percent,
					curdate() as createddate
				FROM ams_loanaccount
				WHERE loantype in('P And I','FLEX','IO') and
				YEAR(settleDate) = last_year and MONTH(settleDate) = last_month and
				lender = templender
				group by lender,loantype
			) as q1
			order by year desc,month desc;
		END LOOP;
		CLOSE cur_loantype;
	END;
	
	DROP TABLE IF EXISTS tempLoanType;

END$$

DELIMITER ;

