
DROP procedure IF EXISTS `sp_portfolio_quality1_securitystate_insert`;

DELIMITER $$

CREATE PROCEDURE `sp_portfolio_quality1_securitystate_insert`()
BEGIN

	DECLARE last_month varchar(2);
	DECLARE last_year varchar(4);
	
	CREATE TEMPORARY TABLE IF NOT EXISTS tempSecurityState
	(
		lender varchar(45),
		numbr int
	);

	SET last_month =(select LPAD(MONTH(curdate() - INTERVAL 1 MONTH),2,'0'));
	SET last_year =(select YEAR(curdate() - INTERVAL 1 MONTH));

	DELETE FROM rpt_portfolio_quality1_securitystate WHERE year=last_year and month=last_month;

	INSERT INTO tempSecurityState
	SELECT q1.lender,sum(q1.total) from (
		select lender,count(*) as total FROM ams_loanaccount
		where loanaccountid in(select loanaccountid from crm_property_loan where 
		propertyregistid in(select propertyregistid from crm_property_register)) and
		YEAR(settleDate) =last_year  and MONTH(settleDate) = last_month group by lender 
		
		UNION ALL
		
		select lender,COUNT(*) as total from ams_loanaccount as a 
		where (select count(*) from crm_property_loan where loanaccountid=a.loanaccountid)=2 and
		YEAR(settleDate) = last_year and MONTH(settleDate) = last_month group by lender
		
		UNION All
		
		select lender,COUNT(*) as total from ams_loanaccount as a 
		where (select count(*) from crm_property_loan where loanaccountid=a.loanaccountid)=3 and
		YEAR(settleDate) = last_year and MONTH(settleDate) = last_month group by lender
		
		UNION All
		
		select lender,COUNT(*) as total from ams_loanaccount as a 
		where (select count(*) from crm_property_loan where loanaccountid=a.loanaccountid)>=4 and
		YEAR(settleDate) = last_year and MONTH(settleDate) = last_month group by lender
	) q1 group by lender;

	BEGIN
		DECLARE tempnumb int;
		DECLARE templender varchar(45);
		DECLARE done int DEFAULT FALSE;
		DECLARE cur_securitystate CURSOR for (select * from tempSecurityState);
		
		DECLARE CONTINUE HANDLER FOR NOT FOUND SET done=TRUE;
		OPEN cur_securitystate;
		
		READ_LOOP: LOOP
			FETCH cur_securitystate INTO templender,tempnumb;
			IF done THEN
				LEAVE READ_LOOP;
			END IF;
		
			INSERT INTO rpt_portfolio_quality1_securitystate(state,lender,year,month,number,percent,createddate)
			SELECT * FROM (
				select 
					(select address_state from crm_property_register where propertyregistid in(select propertyregistid from crm_property_loan where loanaccountid=a.loanaccountid) limit 1) as state,
					lender,
					DATE_FORMAT(settleDate,'%Y') as year,
					DATE_FORMAT(settleDate,'%m') as month,
					COUNT(*) as number,
					(COUNT(*)/tempnumb * 100) as percent,
					curdate() as createddate
				from ams_loanaccount as a 
				where loanaccountid in(select loanaccountid from crm_property_loan where propertyregistid in(select propertyregistid from crm_property_register)) and
				YEAR(settleDate) = last_year and MONTH(settleDate) = last_month and lender=templender
				group by lender
				
				union all
				
				select 
					'Loans with 2 properties' as state,
					lender,
					DATE_FORMAT(settleDate,'%Y') as year,
					DATE_FORMAT(settleDate,'%m') as month,
					COUNT(*) as number,
					(COUNT(*)/tempnumb * 100) as percent,
					curdate() as createddate
				from ams_loanaccount as a 
				where (select count(*) from crm_property_loan where loanaccountid=a.loanaccountid)=2 and
				YEAR(settleDate) = last_year and MONTH(settleDate) = last_month and lender=templender
				group by lender
				
				union all
				
				select 
					'Loans with 3 properties' as state,
					lender,
					DATE_FORMAT(settleDate,'%Y') as year,
					DATE_FORMAT(settleDate,'%m') as month,
					COUNT(*) as number,
					(COUNT(*)/tempnumb * 100) as percent,
					curdate() as createddate
				from ams_loanaccount as a 
				where (select count(*) from crm_property_loan where loanaccountid=a.loanaccountid)=3 and
				YEAR(settleDate) = last_year and MONTH(settleDate) = last_month and lender=templender
				group by lender
				
				union all
				
				select 
					'Loans with 4 properties+' as state,
					lender,
					DATE_FORMAT(settleDate,'%Y') as year,
					DATE_FORMAT(settleDate,'%m') as month,
					COUNT(*) as number,
					(COUNT(*)/tempnumb * 100) as percent,
					curdate() as createddate
				from ams_loanaccount as a 
				where (select count(*) from crm_property_loan where loanaccountid=a.loanaccountid)>=4 and
				YEAR(settleDate) = last_year and MONTH(settleDate) = last_month and lender=templender
				group by lender
			) as q1
			ORDER BY year desc,month desc;
		END LOOP;
		CLOSE cur_securitystate;
	END;
	
	DROP TABLE IF EXISTS tempSecurityState;

END$$

DELIMITER ;

