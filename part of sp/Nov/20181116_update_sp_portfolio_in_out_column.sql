
DROP procedure IF EXISTS `sp_portfolio_in_out_column`;

DELIMITER $$

CREATE PROCEDURE `sp_portfolio_in_out_column`(IN p_lender varchar(300))
BEGIN
	SELECT distinct
		CONCAT(year,month) as yearmonth
	FROM rpt_portfolio_inout
	WHERE (ifnull(p_lender,'') = '' or FIND_IN_SET(lender,p_lender) > 0) 
	ORDER BY year desc,month desc;
END$$

DELIMITER ;

