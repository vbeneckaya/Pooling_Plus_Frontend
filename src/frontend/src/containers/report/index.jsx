import React from 'react';
import ReportPowerBi from 'react-powerbi';

const Report = () => {

    const onEmbedded = (embed) => {
        console.log(`Report embedded: `, embed, this);
    };

    const embedConfig = {
        id: 'f6bfd646-b718-44dc-a378-b73e6b528204',
        embedUrl: 'https://app.powerbi.com/reportEmbed?reportId=f6bfd646-b718-44dc-a378-b73e6b528204&groupId=be8908da-da25-452e-b220-163f52476cdd&config=eyJjbHVzdGVyVXJsIjoiaHR0cHM6Ly9XQUJJLVVTLU5PUlRILUNFTlRSQUwtcmVkaXJlY3QuYW5hbHlzaXMud2luZG93cy5uZXQifQ%3d%3d',
        accessToken: 'H4sIAAAAAAAEACWWxw6shhJE_-VusQQM2ZIX5JwzO3LOMATr_fsbyfvuzamu6vr3j5U-w5wWf_7-I6NnF8lHfVLoPQ727E_fjHWHEqXe18DiPoi0igNNXBKdXpqRY29QUAuY8uU1tkWW7YLp0JbobU9OCtY1DzgoxNduSyhK1A7083NtFFALsZeDoV7cwh30cWIqqkvwZ6N3E5n99utD90p2mLkW-nhdL_vPkZDdpxvBQLdlKd76wVNxzZlGma7F2O0AywL8pmcBDaKIaAtKZrxICq7dmgztRK9kq2YsnynhezHCParzyVOWlDG8zZF6kHPBLOaFSRhhT-wtZqv7gXbByqDAFzvxudDtlC948UCbEN2R2I7QKVlAlBQWbWluC1IOVPK8BhO5Ah3u3RhHFUNA7-bngFvk3axadVGqdpqVQVSeFpGm1GyLHyUj7kjuhTzRSWAgM_23in0x53w53bjuCxqCTPEhrX7gBEEqS7V9zm3BFAyncI4BnaqIrvPSBLwu7lNsa-izs-arH0J2wN4h6I9np73r54IlaCHAtwIVeu7jpd2FdplWpFNdoa2Lfelm6qayvL-n5MCL1Rq1FmH5G5aQturJFiVksH1hHgjQqRLGjOQIVEIwogtqCsv2wqeA5eLqiGUAJAb1iOctI3iK1smTJ2Y-GguuZmtpU8IHKIDCVHTtWRHfxbCZkAbtt8Wikk3dNyrUexmMORY53UzFhscFbbte7uBC9kNtGjAOEL7gFp8035P5Srzeuqe_xyQjjoS70nVPheJ0UeqXHE9RWLii9FMpodNvYJ4oYgxsaQxaisK5iJ4Ogx1YDh8QyqRhUFx9NObLbjxUEIOzLMowozt4gsV-8Wg3PUNjIWlpFSpAy8YWLEYJB_Ys2hLSfdE0fh80otmqyxht2OWfzWTRjbjOZHRMw61NgZBVnXqC_s619PjUuwqYTywPkpM9bnphaUGdBR-hx-cmARd8Dcn3DRc_8g3vj9CpcN8QOqX5OifMWkoQUFoded-8IHeYyoKRtk4S4QG0gVhqhmR3fgAbLoFRlJNxFqll2JrXCy_zE5Bj24ca1LsQD4kAUC07QlmOyFS1tBKT3io1tH2me3ejnkvEm66cZEquHvfYR_8SG0mGOvcegEKYJxa700N52pxtaAKe5u2NBqAk32e1-478POZHjqOhAfpmmZ-wfPxCznCUs6kEgLpXoQTutIpBmbzi07cU9J6PRRJRgeOTanNIiimdLoAUT5A_90MZLZm33qiNpD8ZR2n9d9DAaXzkgzIiEPOYgJEBF97giEqBzYefgjXUY2sGtyUsE3Ol0BSk9ToEd4S0FPg8zYM_n9c6Dv5w6pXUhYDlVmkUwP6V9l1OGk4WuDWj7Zfy6QloGtjlUJXHlhG9wpra7l0a8eoUSJxuxo6H3E2MmiEJOSU9g7MLO91BanJVlpcmjHOzeI3MsFOV9zxdbMvKtKdzjFInwjLzJCkmW5tF7_Ib28YFGChyb6eH5TOZatBpATNEoS1q_AKUxw898Nt4RKzG6u7z5CbzjP37PlSdLkXH-OafAbe1BW_PzWsHAxsfrO8N_sKBdwkPWx7B7wMcXq4C94dzRAezV6TDnOatR9iXF7yqE94vZ1LczkSx-Rgnp6_CIoyzhESKuCzAIJz_OW5xlgPxHZ-ebEmZ352nYYAUoz_RgnEQe4JR5BAPhJ5Hj4jzVQReJPMAudOvIDF11Sn4ZrVSiPkQ7O4CJle-rsHPfdpqeJD0exX5FllXFdlsC8WXr3c1QkdcRwinqc6ZPtgjgrZpsZPUaYg-hHLQoz40HEoLQIdFRXC0L_cdNSofzerQwAjVYHY5dw7XBXD7i5wnNc1774aXilMLbmTXFpK36bUIGgO8FjmZUEzZMGVcTrgQ4qnXZ3ebHcddjWnhRomNrJTi_nNmCZmfsiagz_g-0xp-370wm9Z3RxrWdkuIQ7q5yyIpequ3HsTa2WGarFW_0War78TwyVhHR-2QFO_aoIZNMNDUEPNTDBFyLKIjXI4nq2lLFEiwqA9ZWZkZPkpHVvboh5FAI3OP04VWX6GuLYx1eQ52BZoXJDDYhDSz7s0y_XQ3G0qIiXymXbnHzOMdIOf3tTee9KShzh5YHch4evcqxE4WyT8WnRfm8iqvMsbl-fRLmecBvqv4Nh2GcR7OuwvwW08GfI8_71q_XDubcKnCbeOfiovBNSdpy0CoYJ0aVvi9kKn9znPcnwS2J95yspuoMVUyq45-vTmPe0vkgvvkR06RlEj0iwNlaekoHrXPK9L__PPnrz_s9izHrJbPr6Y4DWiZsocBECUXlu6HMg_Zu5qx1_c1UIxkLNqn9pyNQUGXP1XJcwCUIs1CQid4FVR4QNU3KV-xLxP2dyP2lNToLDy2ypBOqGJuVG5rwrEWA-G5rSgR5WaIX2kwxPz83DzErLCWGFuPfj3xOR6IgnNwpXjQR0mUjL6nAAV1AHg8iNiirl_5cy9__iOYhCB2Ur7ylKKJb2a3lNiOz40maYOmhXsVx52L8WWkqx-ZBX7XFFLyYwI4XdtHEsjkzflF8d5m6MEDaazT8097RkcRrTwvDE1mANoXpHA3T4IxAa06JjOKW-Z-gbrZLEuzirXv89hLbw108Y7H6uQLzPvDUQUywF7_YX6Wptzk4Ed5X41GUVYzd8KWqAvDXoMq-08Mt62n9Di38jdmLlDmX1RxIuFrQA4fyCGMYiqnNQiZ9ShWAjxg6aAxPcgY826Y9p0OmvqN6GwDfT_4tx6mc-qxXcL92mB-ncjIZ0J53ayPbZhgZ6HFvFZ330ErD_vtg6UjyCHoYl8X8Gq32l_w4Zn-uJ16wA_RAlAjfyxRiwVjFAdooiQWC0MgBLu3jUY1Ok9x9UYqE7flLhnWRz51XGZAG91s-thN88rrXpT1nQ1FWazxIr-JfLMZWPBCn0A_fMnRfbNTcMl4zH3kuJXsuQtWmwXrCYkqgeC3IL-nnawPNocW7Tw-Ngquew3zvjkBvKE4Jy0w7HgjDB7KED8zZiIvTH6zVETJQNGAi6h_mP_3f_zlwMeaCwAA'
    };

    console.log('&&&&&&&&&&');

    return (
        <div>
            <h1>react-powerbi demo</h1>

            <ReportPowerBi
                id={embedConfig.id}
                embedUrl={embedConfig.embedUrl}
                accessToken={embedConfig.accessToken}
                filterPaneEnabled={true}
                navContentPaneEnabled={false}
                onEmbedded={onEmbedded}
            />
        </div>
    );
};

export default Report;
