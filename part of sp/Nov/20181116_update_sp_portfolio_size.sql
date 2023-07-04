
DROP procedure IF EXISTS `sp_portfolio_size`;

DELIMITER $$

CREATE PROCEDURE `sp_portfolio_size`(IN p_lender varchar(300))
BEGIN
	SELECT *,
		CONCAT(year,month) as yearmonth
	FROM rpt_portfolio_size
	WHERE (ifnull(p_lender,'') = '' or FIND_IN_SET(lender,p_lender) > 0)
	ORDER BY year desc, month desc;
END$$

DELIMITER ;

