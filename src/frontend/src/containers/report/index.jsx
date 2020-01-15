import React from 'react';
import ReportPowerBi from 'powerbi-report-component';

const Report = () => {

    const onEmbedded = (embed) => {
        console.log(`Report embedded: `, embed, this);
    };

    const embedConfig = {
        id: 'f6bfd646-b718-44dc-a378-b73e6b528204',
        embedUrl: 'https://app.powerbi.com/reportEmbed?reportId=f6bfd646-b718-44dc-a378-b73e6b528204&groupId=be8908da-da25-452e-b220-163f52476cdd&config=eyJjbHVzdGVyVXJsIjoiaHR0cHM6Ly9XQUJJLVVTLU5PUlRILUNFTlRSQUwtcmVkaXJlY3QuYW5hbHlzaXMud2luZG93cy5uZXQifQ%3d%3d',
        accessToken: 'H4sIAAAAAAAEACWWxw6shhJE_-VusQQM2ZIX5JwzO3LOMATr_fsbyfvuzamu6vr3j5U-w5wWf_7-I6NnF8lHfVLoPQ727E_fjHWHEqXe18DiPoi0igNNXBKdXpqRY29QUAuY8uU1tkWW7YLp0JbobU9OCtY1DzgoxNduSyhK1A7083NtFFALsZeDoV7cwh30cWIqqkvwZ6N3E5n99utD90p2mLkW-nhdL_vPkZDdpxvBQLdlKd76wVNxzZlGma7F2O0AywL8pmcBDaKIaAtKZrxICq7dmgztRK9kq2YsnynhezHCParzyVOWlDG8zZF6kHPBLOaFSRhhT-wtZqv7gXbByqDAFzvxudDtlC948UCbEN2R2I7QKVlAlBQWbWluC1IOVPK8BhO5Ah3u3RhHFUNA7-bngFvk3axadVGqdpqVQVSeFpGm1GyLHyUj7kjuhTzRSWAgM_23in0x53w53bjuCxqCTPEhrX7gBEEqS7V9zm3BFAyncI4BnaqIrvPSBLwu7lNsa-izs-arH0J2wN4h6I9np73r54IlaCHAtwIVeu7jpd2FdplWpFNdoa2Lfelm6qayvL-n5MCL1Rq1FmH5G5aQturJFiVksH1hHgjQqRLGjOQIVEIwogtqCsv2wqeA5eLqiGUAJAb1iOctI3iK1smTJ2Y-GguuZmtpU8IHKIDCVHTtWRHfxbCZkAbtt8Wikk3dNyrUexmMORY53UzFhscFbbte7uBC9kNtGjAOEL7gFp8035P5Srzeuqe_xyQjjoS70nVPheJ0UeqXHE9RWLii9FMpodNvYJ4oYgxsaQxaisK5iJ4Ogx1YDh8QyqRhUFx9NObLbjxUEIOzLMowozt4gsV-8Wg3PUNjIWlpFSpAy8YWLEYJB_Ys2hLSfdE0fh80otmqyxht2OWfzWTRjbjOZHRMw61NgZBVnXqC_s619PjUuwqYTywPkpM9bnphaUGdBR-hx-cmARd8Dcn3DRc_8g3vj9CpcN8QOqX5OifMWkoQUFoded-8IHeYyoKRtk4S4QG0gVhqhmR3fgAbLoFRlJNxFqll2JrXCy_zE5Bj24ca1LsQD4kAUC07QlmOyFS1tBKT3io1tH2me3ejnkvEm66cZEquHvfYR_8SG0mGOvcegEKYJxa700N52pxtaAKe5u2NBqAk32e1-478POZHjqOhAfpmmZ-wfPxCznCUs6kEgLpXoQTutIpBmbzi07cU9J6PRRJRgeOTanNIiimdLoAUT5A_90MZLZm33qiNpD8ZR2n9d9DAaXzkgzIiEPOYgJEBF97giEqBzYefgjXUY2sGtyUsE3Ol0BSk9ToEd4S0FPg8zYM_n9c6Dv5w6pXUhYDlVmkUwP6V9l1OGk4WuDWj7Zfy6QloGtjlUJXHlhG9wpra7l0a8eoUSJxuxo6H3E2MmiEJOSU9g7MLO91BanJVlpcmjHOzeI3MsFOV9zxdbMvKtKdzjFInwjLzJCkmW5tF7_Ib28YFGChyb6eH5TOZatBpATNEoS1q_AKUxw898Nt4RKzG6u7z5CbzjP37PlSdLkXH-OafAbe1BW_PzWsHAxsfrO8N_sKBdwkPWx7B7wMcXq4C94dzRAezV6TDnOatR9iXF7yqE94vZ1LczkSx-Rgnp6_CIoyzhESKuCzAIJz_OW5xlgPxHZ-ebEmZ352nYYAUoz_RgnEQe4JR5BAPhJ5Hj4jzVQReJPMAudOvIDF11Sn4ZrVSiPkQ7O4CJle-rsHPfdpqeJD0exX5FllXFdlsC8WXr3c1QkdcRwinqc6ZPtgjgrZpsZPUaYg-hHLQoz40HEoLQIdFRXC0L_cdNSofzerQwAjVYHY5dw7XBXD7i5wnNc1774aXilMLbmTXFpK36bUIGgO8FjmZUEzZMGVcTrgQ4qnXZ3ebHcddjWnhRomNrJTi_nNmCZmfsiagz_g-0xp-370wm9Z3RxrWdkuIQ7q5yyIpequ3HsTa2WGarFW_0War78TwyVhHR-2QFO_aoIZNMNDUEPNTDBFyLKIjXI4nq2lLFEiwqA9ZWZkZPkpHVvboh5FAI3OP04VWX6GuLYx1eQ52BZoXJDDYhDSz7s0y_XQ3G0qIiXymXbnHzOMdIOf3tTee9KShzh5YHch4evcqxE4WyT8WnRfm8iqvMsbl-fRLmecBvqv4Nh2GcR7OuwvwW08GfI8_71q_XDubcKnCbeOfiovBNSdpy0CoYJ0aVvi9kKn9znPcnwS2J95yspuoMVUyq45-vTmPe0vkgvvkR06RlEj0iwNlaekoHrXPK9L__PPnrz_s9izHrJbPr6Y4DWiZsocBECUXlu6HMg_Zu5qx1_c1UIxkLNqn9pyNQUGXP1XJcwCUIs1CQid4FVR4QNU3KV-xLxP2dyP2lNToLDy2ypBOqGJuVG5rwrEWA-G5rSgR5WaIX2kwxPz83DzErLCWGFuPfj3xOR6IgnNwpXjQR0mUjL6nAAV1AHg8iNiirl_5cy9__iOYhCB2Ur7ylKKJb2a3lNiOz40maYOmhXsVx52L8WWkqx-ZBX7XFFLyYwI4XdtHEsjkzflF8d5m6MEDaazT8097RkcRrTwvDE1mANoXpHA3T4IxAa06JjOKW-Z-gbrZLEuzirXv89hLbw108Y7H6uQLzPvDUQUywF7_YX6Wptzk4Ed5X41GUVYzd8KWqAvDXoMq-08Mt62n9Di38jdmLlDmX1RxIuFrQA4fyCGMYiqnNQiZ9ShWAjxg6aAxPcgY826Y9p0OmvqN6GwDfT_4tx6mc-qxXcL92mB-ncjIZ0J53ayPbZhgZ6HFvFZ330ErD_vtg6UjyCHoYl8X8Gq32l_w4Zn-uJ16wA_RAlAjfyxRiwVjFAdooiQWC0MgBLu3jUY1Ok9x9UYqE7flLhnWRz51XGZAG91s-thN88rrXpT1nQ1FWazxIr-JfLMZWPBCn0A_fMnRfbNTcMl4zH3kuJXsuQtWmwXrCYkqgeC3IL-nnawPNocW7Tw-Ngquew3zvjkBvKE4Jy0w7HgjDB7KED8zZiIvTH6zVETJQNGAi6h_mP_3f_zlwMeaCwAA'
    };

    const myStyleObject = {};

    console.log('&&&&&&&&&&');

    return (
        <div className="container">
            <ReportPowerBi embedType="report"
                    tokenType="Embed"
                    accessToken="H4sIAAAAAAAEAC2Wtw6sCBZE_-WlrISnYaUJgMZ7bzK8957R_vu2NJNXdKrq1v37j5k8_ZTkf_77R9EG0UnSNEarHMvo5GOKXxLSraK0MzOj9s6QA4qzjAQvJKSjR2XJDuliTHduJDtsslc_8plEjudpKuzt5qOsGkLpTxKW5enItrKx3rdJ4xSoVKdqmMc8jFY854klKJsGLxKDxMo8gmDyWLw_dvLQI5gk4PaVwN76jp1EC-SJoAha1eAt20G1ccpI3uf9SvLrKDvtM5nOv8-WP1XWsMoYBGcOmowoIW2T5XMpogOlA-j-OGI6x4xra0CmcLpofwjwQY856kWef_J1iafAVFEs2rIS8zQH_6BvN2TOmfPfR2pvPjKC8mrWMRugZRx6GNED4aCSDhKSbtaYe4VztmVTv_fCz1zMYa-LMY3jSxW4el13E9h8s0khIV74Xn2sfb7ecZDm4JVMub0Qn2EnSBfVF2ccbif7dDibRBlTXcColViHR1rrEHSVJFVKnGi5rn64fbpKsYBFsPByGOKNbK64ea_Ix_tQGKBNVn1I-asW5XN5ZnVsPC21wcz7e1cymby-EVOe7Q1iJNQrPXVD5qXUi-5VyCLyOb4GJA9nvJQfcfcGhNx-YcmsiyymZC8YpgpMOY2yCrG5TzsemNEbNB6dWazRCZb-uUDH-87d2ZzaBfHxl4No-xL3Ox4tGQ4XVeEaF8wxnegeVxy8QyMERJ0hTbS7iXevTMsmg-lJcMJRT-nTZ05Af9vs7F38OAcPsdf7yuKTyUPS4kBOdZAApr6UNYIVUr6LI9R10rplkiTCVXR2S0Q3UsYLYrORESpzBGzgOe0L3hnrkLcV1pYD9MxEjGwzuLHZ6pryOPvBC460RNKLXDf3WqhpnSWC_2nghTIooCefmVaXJcZhGuy_I1Z845Eu5RX2JIm8c3UNbx4lBR6A8XVbOQPK44jdDFGraSoGHxoN4l61-amv2WvyT77A5pgzcIeuqVr1IWi8GBlYXHN3ft2zPsyXAwu87qM3ql15YJdpuZRBBgPRmX0QrqA2OTjfXOjsSgVw1xPch3KpZdqRYUpdQjt1b4B95CjPrhgFCcSuOALqsQOXgmo9TbahLYNhlM9Ehp2V3o2RynsPUK6I5ItyEbBx4SlahFqfZ69AnCgjkVZ0V4ZLF7PtOXw3C-S8T4kZzNSf-z5Me1qndA9pJ_HIBT96OEdSqNrEJ5xwnCYXF6FGisEQfodBKwslyQiKpot68V44IE2qTOyWE13dyp6u-8dxHg1tp0rWhSuBQyW9Qkb6tpIXGyA2HpxNSDlxti_GpuBLQ5eXFI1udgbbgocrvmgmSEwER2nFMzGTRYquJZUhlHRIxrIT9Q8zC7MKOBUkDncUauMmI-nFXCEKeYg3Oq1yjuoF2htlbC7ILeYWPCQo5_Y-CMU7oztI-ufXyG8wTTiiKhUvx2HoCTEKXAXSO8ew-VQc27OkOgnpjDtONkSncXyBBP9A-MS-NGrh6yxoqsvcmUOO43FS2x266OTTNbzv2Oiz5goE2WlawSizHxHZU_F7ufTMuglcXMwqu8FGeu5AkxYB_ZKlVDuKoySNXb0NTjpHdlGS8JhaLxPyJe78eZbD7LrP5kcEY80HuJZFO6ynL7ryp8cq2mezTpOHlNM_wGm-AZIYRBW636bsQcrMzlCFscNLfbgJsKBJLlYVsSCBIXLTCO81e4PfYOGGKdXZfLeuv4kF6O6pxyIezBCEBOs1XSuwn14sxwLYuIfFg61SIau7WpJeV2LJK_Huazuing1tHB3Hrt7dHJYN1Yf_sZam4T9zFZCOn79lgL8d2mlESEbPoRVl25hramGvwHRJ2kN8uy8-PbiyrhubsWOIFC6oNNnPQ3igL3_V1g7m2bB5ZbpXEU-0Oy8SGMWFQF_XcpR5zIdkNTjmvkXgfGzQbpU9Slb4gRPKr_kzOqxXJ3Vg9F5QADJzvllq4tysBy19qotxf2bWLJQ2dphUx84n9qAsEJFe0bhKoOlXDJkWPktrtFp0zTa734B4LsP0z93xwKTXTzZnxo4Wb1BrCwtBOufK0Izdv0SbF1o7wqU9zW-AlA_E-LOkuSHo7aH-ItXv9tnAWp1kTADHQAINfBNk29Bt_303_7VV6LRUa0XcNKKR4TvhcZ_r8hzoPsJEQV-FM8VEYendpKZEQBI28yic6Jgb9et9iMlYVULyDmjoF1CFYEIYnVWJswYrP6XujaRuvUBPyIOqu8GAKwD9epb8ooNEqTszXhAWhmHqmOIcS9Vff_35zx92feZ9Uorn96b4KkiRSK6LqHV05izf6KHIpQ6k0OfDCDhN37K6ZQbd5H6zSruitre6HHlyq2gGfVCt5RX5V9r0LvPj1qARs0SL844pH_Du9Mok-vItPG1hYqlsr7_3l-bdO4sFCi4HeAy5rdvjeRfm3VG6DyOl0EEEyXfeEVHvOeK93HJvihhoFGzOqJNLNGZ3dMeMasfYikUS7B1fwmaRTLA9rKPHZ1I6Me_CVvke1pUyKkesdroGIRzbPnA8irWo2geHiEYmqO1C0TZ2mnGSUQ8eg86Vb5BUg13wtlVC17jYmDeczkEHlpjPhD--24SotursK1A71NUJNFqPnGl9VZnaTMF3mLWKtX8xP3NdrJL_o7yNX9xqtpzd8s_O47knCPr7j8ppqjHZj7X4yfrpc9wvYzp7hTbpb4aa-ciFJ4xzGyzWcYRbkwALJG0iZdf9hpgafSKuD53UbqbaGsMkz660SCgSMDZKZm_H5qnLhz4ps6HLd5ExKxDNcmCcpfwIjUgfCClRBNTHtFolY6vna7hzvo6cmAi288jxuFOf9Nw_pPtkXMFmOILN1RO1hgWaup1kqQtkdv1KwhvlIm84E2hM4bsBV6tv5v4SH_q3tjLiDUTKLoeGDHPW7PaZRFVOCHOgwP0GmtTD49NLFmGZ9dsLyGuqmqZhaJrMl5m0VTbtpO2mrjcHC_t5T7UIHTLnnPzbKLWtGt7Ps9RSIoUgIkKnoxcOca6pBX_N4E96_TD_7__yWOhwmgsAAA=="
                    embedUrl="https://app.powerbi.com/reportEmbed?reportId=f6bfd646-b718-44dc-a378-b73e6b528204&groupId=be8908da-da25-452e-b220-163f52476cdd&config=eyJjbHVzdGVyVXJsIjoiaHR0cHM6Ly9XQUJJLVVTLU5PUlRILUNFTlRSQUwtcmVkaXJlY3QuYW5hbHlzaXMud2luZG93cy5uZXQifQ%3d%3d"
                    embedId="f6bfd646-b718-44dc-a378-b73e6b528204"
                    dashboardId=""
                    pageName=""
                    extraSettings={{
                        filterPaneEnabled: false,
                        navContentPaneEnabled: false,
                    }}
                    permissions="All"
                    style={{
                        height: 'calc(100vh - 60px)',
                        width: '100%',
                        border: '0',
                        padding: '20px',
                        background: '#eee'
                    }}
                    onLoad={(report) => {
                        /*
                        you can set filters onLoad using:
                        this.report.setFilters([filter]).catch((errors) => {
                          console.log(errors);
                        });*/
                        console.log('Report Loaded!');
                        //this.report = report (Read docs to know how to use report object that is returned)
                    }}
                    onSelectData={(data) => {
                        window.alert('You clicked chart:' + data.visual.title);
                    }}
                    onPageChange={(data) => {
                        console.log('You changed page to:' + data.newPage.displayName);
                    }}
                    onTileClicked={data => {
                        console.log('Data from tile', data);
                    }}
            />
        </div>
    );
};

export default Report;
