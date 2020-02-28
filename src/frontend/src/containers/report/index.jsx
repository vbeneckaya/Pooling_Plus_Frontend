import React, { useEffect, useRef, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import ReportPowerBi from 'powerbi-report-component';
import { getReportRequest, reportSelector } from '../../ducks/reports';

const Report = () => {
    const dispatch = useDispatch();

    const containerRef = useRef(null);

    let [isMobile, setIsMobile] = useState(false);

    const report = useSelector(state => reportSelector(state));

    useEffect(() => {
        dispatch(getReportRequest());
    }, []);

    useEffect(
        () => {
            if (containerRef && containerRef.current && containerRef.current.offsetWidth <= 990) {
                setIsMobile(true);
            } else {
                setIsMobile(false);
            }
        },
        [containerRef, containerRef.current],
    );

    console.log('isMobile', isMobile);

    return (
        <div className="report-container" ref={containerRef}>
            <ReportPowerBi
                embedType="report"
                tokenType="Embed"
                accessToken={report.token}
                embedUrl={report.embedURL}
                embedId={report.reportID}
                dashboardId=""
                pageName={isMobile ? 'ReportSection3b6892f9816b68708ab6' : ''}
                extraSettings={{
                    filterPaneEnabled: !isMobile,
                    navContentPaneEnabled: !isMobile,
                    layoutType: isMobile ? 2 : 0,
                }}
                permissions="All"
                style={{
                    height: 'calc(100vh - 40px)',
                    width: '100%',
                    border: '0',
                    background: '#eee',
                }}
                onLoad={report => {}}
                onSelectData={data => {}}
                onPageChange={data => {}}
                onTileClicked={data => {}}
            />
        </div>
    );
};

export default Report;
